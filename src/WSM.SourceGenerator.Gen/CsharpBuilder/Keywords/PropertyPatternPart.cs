namespace SourceGenerator.CsharpBuilder.Keywords;
public class PropertyPatternPart : ICSBuilderPart
{
    public string Name { get; }
    public string Type { get; }
    public string DefaultValue { get; }
    public ProtectionTypeEnum Protection { get; }
    public string GetterValue { get; }
    public string SetterValue { get; }
    public bool DisableGetter { get; }
    public bool DisableSetter { get; }
    public PropertyPatternPart(string name, string type, string defaultValue = null, ProtectionTypeEnum protection = ProtectionTypeEnum.Public, string getterValue = null, string setterValue = null, bool disableGetter = false, bool disableSetter = false)
    {
        Name = name;
        Type = type;
        DefaultValue = defaultValue;
        Protection = protection;
        GetterValue = getterValue.Trim();
        SetterValue = setterValue.Trim();
        DisableGetter = disableGetter;
        DisableSetter = disableSetter;
    }

    public StringBuilder Build(StringBuilder builder)
    {
        builder.Append(Protection.ToString().ToLower());
        builder.Append($" {Type} {Name}");
        if (DisableGetter && DisableSetter)
            throw new InvalidOperationException("Property Can't Be Without get or set");
        builder.Append("{ ");
        if (!DisableGetter)
        {
            if (string.IsNullOrEmpty(GetterValue))
            {
                builder.Append("get;");
            }
            else
            {
                builder.Append($"get {GetterValue}");
                if (!GetterValue.EndsWith("}"))
                    builder.Append($";");
            }
        }
        if (!DisableSetter)
        {
            if (string.IsNullOrEmpty(SetterValue))
            {
                builder.Append("set;");
            }
            else
            {
                builder.Append($"set {SetterValue}");
                if (!SetterValue.EndsWith("}"))
                    builder.Append($";");
            }
        }
        builder.Append(" }");
        if (!string.IsNullOrEmpty(DefaultValue))
            builder.Append($" = {DefaultValue}");
        return builder;
    }
}
