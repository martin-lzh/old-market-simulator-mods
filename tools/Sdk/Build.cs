using Mono.Cecil;
using Mono.Cecil.Cil;

static class Build
{
    sealed class MemoryResolver : DefaultAssemblyResolver {
        public void Add(AssemblyDefinition a) => RegisterAssembly(a);
    }
    public static void Run(Api api, string output) {
        Directory.CreateDirectory(output);
        using var resolver = new MemoryResolver();
        var assemblies = api.Assemblies.ToDictionary(a => a.Name, a => {
            var n = AssemblyNameReference.Parse(a.Name);
            return AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(n.Name, n.Version) {
                Culture = n.Culture, PublicKeyToken = n.PublicKeyToken
            }, n.Name, new ModuleParameters { Kind = ModuleKind.Dll, AssemblyResolver = resolver });
        });
        foreach (var a in assemblies.Values) resolver.Add(a);
        var types = new Dictionary<string, TypeDefinition>();
        foreach (var t in api.Types.OrderBy(t => t.Name.Count(c => c == '/'))) {
            var module = assemblies[t.Assembly].MainModule;
            var nested = t.Name.LastIndexOf('/');
            var dot = t.Name.LastIndexOf('.');
            var name = nested >= 0 ? t.Name[(nested + 1)..] : t.Name[(dot + 1)..];
            var ns = nested >= 0 || dot < 0 ? "" : t.Name[..dot];
            var td = new TypeDefinition(ns, name, (TypeAttributes)t.Attributes);
            if (t.Base?.Name is "System.ValueType" or "System.Enum")
                td.BaseType = new TypeReference("System", t.Base.Name[7..], module, module.TypeSystem.CoreLibrary);
            if (nested >= 0) types[t.Assembly + "|" + t.Name[..nested]].NestedTypes.Add(td);
            else module.Types.Add(td);
            types.Add(t.Assembly + "|" + t.Name, td);
            foreach (var g in t.Generics) td.GenericParameters.Add(new GenericParameter(g.Name, td) { Attributes = (GenericParameterAttributes)g.Attributes });
        }
        foreach (var t in api.Types) {
            var td = types[t.Assembly + "|" + t.Name];
            var module = td.Module;
            TypeReference Resolve(Ref r, MethodDefinition? method = null) {
                switch (r.Kind) {
                    case "type": return td.GenericParameters[r.Position];
                    case "method": return method!.GenericParameters[r.Position];
                    case "generic":
                        var gi = new GenericInstanceType(Resolve(r.Element!, method));
                        foreach (var arg in r.Arguments!) gi.GenericArguments.Add(Resolve(arg, method));
                        return gi;
                    case "array": return new ArrayType(Resolve(r.Element!, method), r.Rank);
                    case "byref": return new ByReferenceType(Resolve(r.Element!, method));
                    case "pointer": return new PointerType(Resolve(r.Element!, method));
                    case "optional": return new OptionalModifierType(Resolve(r.Arguments![0], method), Resolve(r.Element!, method));
                    case "required": return new RequiredModifierType(Resolve(r.Arguments![0], method), Resolve(r.Element!, method));
                    case "named":
                        var primitive = r.Name switch {
                            "System.Void" => module.TypeSystem.Void, "System.Object" => module.TypeSystem.Object,
                            "System.String" => module.TypeSystem.String, "System.Boolean" => module.TypeSystem.Boolean,
                            "System.Char" => module.TypeSystem.Char, "System.SByte" => module.TypeSystem.SByte,
                            "System.Byte" => module.TypeSystem.Byte, "System.Int16" => module.TypeSystem.Int16,
                            "System.UInt16" => module.TypeSystem.UInt16, "System.Int32" => module.TypeSystem.Int32,
                            "System.UInt32" => module.TypeSystem.UInt32, "System.Int64" => module.TypeSystem.Int64,
                            "System.UInt64" => module.TypeSystem.UInt64, "System.Single" => module.TypeSystem.Single,
                            "System.Double" => module.TypeSystem.Double, "System.IntPtr" => module.TypeSystem.IntPtr,
                            "System.UIntPtr" => module.TypeSystem.UIntPtr, _ => null
                        };
                        if (primitive != null) return primitive;
                        if (types.TryGetValue(r.Assembly + "|" + r.Name, out var known)) return module.ImportReference(known);
                        var scope = module.AssemblyReferences.FirstOrDefault(a => a.FullName == r.Assembly);
                        if (scope == null) {
                            scope = AssemblyNameReference.Parse(r.Assembly);
                            module.AssemblyReferences.Add(scope);
                        }
                        TypeReference Named(string full) {
                            var slash = full.LastIndexOf('/');
                            if (slash >= 0) return new TypeReference("", full[(slash + 1)..], module, scope, r.ValueType) { DeclaringType = Named(full[..slash]) };
                            var dot = full.LastIndexOf('.');
                            return new TypeReference(dot < 0 ? "" : full[..dot], full[(dot + 1)..], module, scope, r.ValueType);
                        }
                        return module.ImportReference(Named(r.Name));
                    default: throw new InvalidOperationException(r.Kind);
                }
            }
            void Constraints(List<Generic> gs, IGenericParameterProvider p, MethodDefinition? m = null) {
                for (int i = 0; i < gs.Count; i++) {
                    foreach (var c in gs[i].Constraints) p.GenericParameters[i].Constraints.Add(new GenericParameterConstraint(Resolve(c, m)));
                    if (gs[i].Unmanaged) Attribute(p.GenericParameters[i], module, "System.Runtime.CompilerServices", "IsUnmanagedAttribute");
                }
            }
            ParameterDefinition Param(Parameter p, MethodDefinition? m = null) {
                var result = new ParameterDefinition(p.Name, (ParameterAttributes)p.Attributes, Resolve(p.Type, m));
                if ((p.Attributes & (int)ParameterAttributes.HasDefault) != 0) result.Constant = Constant(p.Constant, p.ConstantType);
                if (p.ParamArray) Attribute(result, module, "System", "ParamArrayAttribute");
                return result;
            }
            td.BaseType = t.Base == null ? null : Resolve(t.Base);
            foreach (var i in t.Interfaces) td.Interfaces.Add(new InterfaceImplementation(Resolve(i)));
            Constraints(t.Generics, td);
            if (t.Extension) Attribute(td, module, "System.Runtime.CompilerServices", "ExtensionAttribute");
            if (t.DefaultMember != null) {
                var ctor = new MethodReference(".ctor", module.TypeSystem.Void,
                    new TypeReference("System.Reflection", "DefaultMemberAttribute", module, module.TypeSystem.CoreLibrary)) { HasThis = true };
                ctor.Parameters.Add(new ParameterDefinition(module.TypeSystem.String));
                var attribute = new CustomAttribute(ctor);
                attribute.ConstructorArguments.Add(new CustomAttributeArgument(module.TypeSystem.String, t.DefaultMember));
                td.CustomAttributes.Add(attribute);
            }
            foreach (var m in t.Methods) {
                var md = new MethodDefinition(m.Name, (MethodAttributes)m.Attributes, module.TypeSystem.Void);
                td.Methods.Add(md);
                foreach (var g in m.Generics) md.GenericParameters.Add(new GenericParameter(g.Name, md) { Attributes = (GenericParameterAttributes)g.Attributes });
                md.ReturnType = Resolve(m.ReturnType, md);
                foreach (var p in m.Parameters) md.Parameters.Add(Param(p, md));
                Constraints(m.Generics, md, md);
                if (m.Extension) Attribute(md, module, "System.Runtime.CompilerServices", "ExtensionAttribute");
                md.IsPInvokeImpl = false;
                if (!md.IsAbstract) {
                    // Compiler-only shell. This body is authored here, never copied from the game.
                    md.Body.Instructions.Add(Instruction.Create(OpCodes.Ldnull));
                    md.Body.Instructions.Add(Instruction.Create(OpCodes.Throw));
                }
            }
            foreach (var f in t.Fields.OrderBy(f => f.Name == "value__" ? 0 : 1)) {
                var fd = new FieldDefinition(f.Name, (FieldAttributes)f.Attributes, Resolve(f.Type));
                if ((f.Attributes & (int)FieldAttributes.HasDefault) != 0) fd.Constant = Constant(f.Constant, f.ConstantType);
                td.Fields.Add(fd);
            }
            foreach (var p in t.Properties) {
                var pd = new PropertyDefinition(p.Name, (PropertyAttributes)p.Attributes, Resolve(p.Type));
                foreach (var param in p.Parameters) pd.Parameters.Add(Param(param));
                if (p.Getter >= 0) pd.GetMethod = td.Methods[p.Getter];
                if (p.Setter >= 0) pd.SetMethod = td.Methods[p.Setter];
                td.Properties.Add(pd);
            }
            foreach (var e in t.Events) td.Events.Add(new EventDefinition(e.Name, (EventAttributes)e.Attributes, Resolve(e.Type)) {
                AddMethod = e.Add < 0 ? null : td.Methods[e.Add], RemoveMethod = e.Remove < 0 ? null : td.Methods[e.Remove]
            });
        }
        foreach (var a in assemblies.Values) {
            foreach (var f in api.Assemblies.Single(x => x.Name == a.Name.FullName).Forwarders) {
                var dot = f.Name.LastIndexOf('.');
                var scope = AssemblyNameReference.Parse(f.Assembly);
                a.MainModule.AssemblyReferences.Add(scope);
                a.MainModule.ExportedTypes.Add(new ExportedType(f.Name[..dot], f.Name[(dot + 1)..], a.MainModule, scope) { IsForwarder = true });
            }
            Attribute(a, a.MainModule, "System.Runtime.CompilerServices", "ReferenceAssemblyAttribute");
            if (a.MainModule.Types.Any(t => t.CustomAttributes.Any(c => c.AttributeType.Name == "ExtensionAttribute")))
                Attribute(a, a.MainModule, "System.Runtime.CompilerServices", "ExtensionAttribute");
            a.Write(Path.Combine(output, a.Name.Name + ".dll"), new WriterParameters { DeterministicMvid = true });
            a.Dispose();
        }
    }
    static void Attribute(ICustomAttributeProvider target, ModuleDefinition m, string ns, string name) {
        var t = new TypeReference(ns, name, m, m.TypeSystem.CoreLibrary);
        target.CustomAttributes.Add(new CustomAttribute(new MethodReference(".ctor", m.TypeSystem.Void, t) { HasThis = true }));
    }
    static object? Constant(string? value, string? type) => type switch {
        null => null, "String" => value, "Char" => value![0],
        _ => Convert.ChangeType(value, System.Type.GetType("System." + type)!, System.Globalization.CultureInfo.InvariantCulture)
    };
}
