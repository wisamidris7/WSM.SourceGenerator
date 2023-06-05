namespace SourceGenerator.CsharpBuilder.Keywords;
public class ClassPatternPart : ICSBuilderPart
{
    public ClassPatternPart(ICSBuilder body, string name, ClassTypeEnum type = ClassTypeEnum.Normal, ProtectionTypeEnum protectionType = ProtectionTypeEnum.Public, string? inherits = null, bool @static = false)
    {
        Body = body;
        Name = name;
        Type = type;
        ProtectionType = protectionType;
        Inherits = inherits;
        Static = @static;
    }

    public ICSBuilder Body { get; }
    public string Name { get; }
    public ClassTypeEnum Type { get; }
    public ProtectionTypeEnum ProtectionType { get; }
    public string? Inherits { get; }
    public bool Static { get; }

    public StringBuilder Build(StringBuilder builder)
    {
        builder.Append(ProtectionType.ToString().ToLower());
        if (Static)
            builder.Append($" static ");

        if (Type != ClassTypeEnum.Normal)
            builder.Append($" {Type.ToString().ToLower()}");
        builder.AppendLine($" class {Name}");
        if (!string.IsNullOrEmpty(Inherits))
            builder.AppendLine($" : {Inherits}");
        builder.AppendLine("{");
        builder.AppendLine(Body.Build().ToString());
        builder.AppendLine("}");
        return builder;
    }
}
public enum ClassTypeEnum
{
    Normal,
    Partial,
    Abstract
}
