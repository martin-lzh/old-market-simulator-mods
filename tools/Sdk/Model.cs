using Mono.Cecil;

// Declarative API metadata only: no IL, resources, field initializers or game strings.
record Api(List<ApiAssembly> Assemblies, List<ApiType> Types);
record ApiAssembly(string Name, string Sha256, List<Ref> Forwarders);
record Ref(string Kind, string Name = "", string Assembly = "", bool ValueType = false,
    Ref? Element = null, List<Ref>? Arguments = null, int Position = 0, int Rank = 0);
record Generic(string Name, int Attributes, List<Ref> Constraints, bool Unmanaged);
record Parameter(string Name, int Attributes, Ref Type, string? Constant, string? ConstantType, bool ParamArray);
record Method(string Name, int Attributes, Ref ReturnType, List<Parameter> Parameters,
    List<Generic> Generics, bool Extension);
record Field(string Name, int Attributes, Ref Type, string? Constant, string? ConstantType);
record Property(string Name, int Attributes, Ref Type, List<Parameter> Parameters, int Getter, int Setter);
record Event(string Name, int Attributes, Ref Type, int Add, int Remove);
record ApiType(string Assembly, string Name, int Attributes, Ref? Base, List<Ref> Interfaces,
    List<Generic> Generics, List<Method> Methods, List<Field> Fields, List<Property> Properties,
    bool Extension, string? DefaultMember, List<Event> Events);

static class Metadata
{
    public static string Identity(TypeReference t) => t.Scope switch {
        AssemblyNameReference a => a.FullName,
        ModuleDefinition m => m.Assembly.Name.FullName,
        _ => throw new NotSupportedException($"Type scope: {t.Scope}")
    };
    public static Ref Type(TypeReference t) => t switch {
        GenericParameter g => new(g.Type == GenericParameterType.Method ? "method" : "type", Position: g.Position),
        GenericInstanceType g => new("generic", Element: Type(g.ElementType), Arguments: g.GenericArguments.Select(Type).ToList()),
        ArrayType a => new("array", Element: Type(a.ElementType), Rank: a.Rank),
        ByReferenceType b => new("byref", Element: Type(b.ElementType)),
        PointerType p => new("pointer", Element: Type(p.ElementType)),
        OptionalModifierType m => new("optional", Element: Type(m.ElementType), Arguments: [Type(m.ModifierType)]),
        RequiredModifierType m => new("required", Element: Type(m.ElementType), Arguments: [Type(m.ModifierType)]),
        TypeSpecification => throw new NotSupportedException(t.FullName),
        _ => new("named", t.FullName, Identity(t), t.IsValueType)
    };
    public static List<Generic> Generics(IGenericParameterProvider p) => p.GenericParameters
        .Select(g => new Generic(g.Name, (int)g.Attributes, g.Constraints.Select(c => Type(c.ConstraintType)).ToList(),
            g.CustomAttributes.Any(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.IsUnmanagedAttribute"))).ToList();
    public static bool Extension(ICustomAttributeProvider p) => p.CustomAttributes.Any(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.ExtensionAttribute");
    public static (string?, string?) Constant(object? v) => v == null ? (null, null) :
        (Convert.ToString(v, System.Globalization.CultureInfo.InvariantCulture), v.GetType().Name);
    public static Parameter Param(ParameterDefinition p) {
        var (value, type) = Constant(p.HasConstant ? p.Constant : null);
        return new(p.Name, (int)p.Attributes, Type(p.ParameterType), value, type,
            p.CustomAttributes.Any(a => a.AttributeType.FullName == "System.ParamArrayAttribute"));
    }
}
