﻿namespace SourceGenerator.CsharpBuilder.Keywords;
public class ParameterPatternPart : ICSBuilderPart
{
    public string Name { get; }
    public string Type { get; }
    public string DefaultValue { get; }
    public ParameterPatternPart(string name, string type, string defaultValue = null)
    {
        Name = name;
        Type = type;
        DefaultValue = defaultValue;
    }

    public StringBuilder Build(StringBuilder builder)
    {
        builder.Append($"{Type} {Name}");
        if (!string.IsNullOrEmpty(DefaultValue))
            builder.Append($" = {DefaultValue}");
        return builder;
    }
}
