using Mono.Cecil;
using System.Security.Cryptography;

sealed class Export
{
    readonly DefaultAssemblyResolver resolver = new();
    readonly Dictionary<string, AssemblyDefinition> assemblies = new();
    readonly HashSet<TypeDefinition> types = [];
    readonly HashSet<MethodDefinition> methods = [];
    readonly HashSet<FieldDefinition> fields = [];
    readonly HashSet<string> visited = [];

    public Export(string managed, IEnumerable<string> dependencies) {
        resolver.AddSearchDirectory(managed);
        foreach (var d in dependencies) resolver.AddSearchDirectory(d);
        foreach (var file in Directory.GetFiles(managed, "*.dll").Order()) {
            var name = Path.GetFileNameWithoutExtension(file);
            if (name == "Assembly-CSharp" || name.StartsWith("Unity")) {
                var a = AssemblyDefinition.ReadAssembly(file, new ReaderParameters { AssemblyResolver = resolver });
                assemblies.Add(a.Name.FullName, a);
            }
        }
    }

    bool Owned(TypeDefinition t) => assemblies.ContainsKey(t.Module.Assembly.Name.FullName);
    void Visit(TypeReference? r) {
        if (r == null || r is GenericParameter) return;
        if (r is GenericInstanceType gi) foreach (var a in gi.GenericArguments) Visit(a);
        if (r is IModifierType modifier) Visit(modifier.ModifierType);
        if (r is TypeSpecification spec) { Visit(spec.ElementType); return; }
        if (!visited.Add(Metadata.Identity(r) + "|" + r.FullName)) return;
        if (r.Namespace.StartsWith("System") || r.Namespace == "Mono") return;
        var t = r.Resolve() ?? throw new InvalidOperationException($"Unresolved type: {r}");
        if (Owned(t)) types.Add(t);
        Visit(t.DeclaringType);
        Visit(t.BaseType);
        foreach (var i in t.Interfaces) Visit(i.InterfaceType);
        foreach (var g in t.GenericParameters) foreach (var c in g.Constraints) Visit(c.ConstraintType);
        // Enum literals are inlined by C#. Delegates need their constructor and Invoke signature.
        if (Owned(t) && (t.IsEnum || t.BaseType?.FullName == "System.MulticastDelegate" || t.IsInterface)) {
            foreach (var f in t.Fields) Add(f);
            foreach (var m in t.Methods) Add(m);
        }
    }
    void Add(FieldDefinition f) {
        if (!Owned(f.DeclaringType) || !fields.Add(f)) return;
        Visit(f.DeclaringType); Visit(f.FieldType);
    }
    void Add(MethodDefinition m) {
        if (!Owned(m.DeclaringType) || !methods.Add(m)) return;
        Visit(m.DeclaringType); Visit(m.ReturnType);
        foreach (var p in m.Parameters) Visit(p.ParameterType);
        foreach (var g in m.GenericParameters) foreach (var c in g.Constraints) Visit(c.ConstraintType);
        // Keep the property pair: C# assignment and compound expressions resolve both accessors.
        foreach (var p in m.DeclaringType.Properties.Where(p => p.GetMethod == m || p.SetMethod == m)) {
            if (p.GetMethod != null) Add(p.GetMethod);
            if (p.SetMethod != null) Add(p.SetMethod);
        }
    }
    public Api Run(IEnumerable<string> plugins, Dictionary<string, string[]> supplemental) {
        foreach (var path in plugins) {
            using var a = AssemblyDefinition.ReadAssembly(path, new ReaderParameters { AssemblyResolver = resolver });
            foreach (var t in a.MainModule.GetTypeReferences()) Visit(t);
            foreach (var member in a.MainModule.GetMemberReferences()) {
                switch (member) {
                    case MethodReference m: Add(m.Resolve() ?? throw new InvalidOperationException(m.FullName)); break;
                    case FieldReference f: Add(f.Resolve() ?? throw new InvalidOperationException(f.FullName)); break;
                }
            }
        }
        foreach (var (name, members) in supplemental) {
            var type = resolver.Resolve(assemblies.Values.Single(a => a.Name.Name == "Assembly-CSharp").Name).MainModule.GetType(name)
                ?? throw new InvalidOperationException($"Supplemental type missing: {name}");
            foreach (var member in members) {
                var selected = type.Methods.Where(m => m.Name == member).ToArray();
                if (selected.Length == 0) throw new InvalidOperationException($"Supplemental member missing: {name}.{member}");
                foreach (var m in selected) Add(m);
            }
        }
        var result = new List<ApiType>();
        foreach (var t in types.OrderBy(t => t.Module.Assembly.Name.Name).ThenBy(t => t.FullName, StringComparer.Ordinal)) {
            var ms = t.Methods.Where(methods.Contains).OrderBy(m => m.FullName, StringComparer.Ordinal).ToList();
            result.Add(new(t.Module.Assembly.Name.FullName, t.FullName, (int)t.Attributes,
                t.BaseType == null ? null : Metadata.Type(t.BaseType),
                t.Interfaces.Select(i => Metadata.Type(i.InterfaceType)).ToList(), Metadata.Generics(t),
                ms.Select(m => new Method(m.Name, (int)m.Attributes, Metadata.Type(m.ReturnType),
                    m.Parameters.Select(Metadata.Param).ToList(), Metadata.Generics(m), Metadata.Extension(m))).ToList(),
                t.Fields.Where(fields.Contains).OrderBy(f => f.Name, StringComparer.Ordinal).Select(f => {
                    // Only enum constants and parameter defaults are needed for source compilation.
                    if (f.HasConstant && !t.IsEnum) throw new InvalidOperationException($"Review constant explicitly: {f.FullName}");
                    var (value, type) = Metadata.Constant(f.HasConstant ? f.Constant : null);
                    return new Field(f.Name, (int)f.Attributes, Metadata.Type(f.FieldType), value, type);
                }).ToList(),
                t.Properties.Where(p => ms.Contains(p.GetMethod) || ms.Contains(p.SetMethod))
                    .Select(p => new Property(p.Name, (int)p.Attributes, Metadata.Type(p.PropertyType),
                        p.Parameters.Select(Metadata.Param).ToList(), ms.IndexOf(p.GetMethod), ms.IndexOf(p.SetMethod))).ToList(),
                Metadata.Extension(t),
                t.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == "System.Reflection.DefaultMemberAttribute")?.ConstructorArguments[0].Value as string,
                t.Events.Where(e => ms.Contains(e.AddMethod) || ms.Contains(e.RemoveMethod))
                    .Select(e => new Event(e.Name, (int)e.Attributes, Metadata.Type(e.EventType), ms.IndexOf(e.AddMethod), ms.IndexOf(e.RemoveMethod))).ToList()));
        }
        // Empty Unity facade references are retained because the projects explicitly reference them.
        var used = result.Select(t => t.Assembly).ToHashSet();
        foreach (var a in assemblies.Values.Where(a => a.Name.Name is "UnityEngine" or "UnityEngine.UIModule" or "UnityEngine.TextRenderingModule")) used.Add(a.Name.FullName);
        return new(assemblies.Values.Where(a => used.Contains(a.Name.FullName)).OrderBy(a => a.Name.Name)
            .Select(a => new ApiAssembly(a.Name.FullName, Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(a.MainModule.FileName))),
                a.MainModule.ExportedTypes.Where(e => types.Any(t => t.FullName == e.FullName))
                    .Select(e => Metadata.Type(e.Resolve())).ToList())).ToList(), result);
    }
}
