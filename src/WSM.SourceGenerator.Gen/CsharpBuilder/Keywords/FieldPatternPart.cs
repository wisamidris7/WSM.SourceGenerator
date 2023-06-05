namespace SourceGenerator.CsharpBuilder.Keywords;
public class FieldPatternPart : ICSBuilderPart
{
    public string Name { get; }
    public string Type { get; }
    public string DefaultValue { get; }
    public ProtectionTypeEnum Protection { get; }
    public FieldPatternPart(string name, string type, string defaultValue = null, ProtectionTypeEnum protection = ProtectionTypeEnum.Public)
    {
        Name = name;
        Type = type;
        DefaultValue = defaultValue;
        Protection = protection;
    }

    public StringBuilder Build(StringBuilder builder)
    {
        builder.Append(Protection.ToString().ToLower());
        builder.Append($" {Type} {Name}");
        if (!string.IsNullOrEmpty(DefaultValue))
            builder.Append($" = {DefaultValue}");
        builder.Append(";");
        return builder;
    }
}
