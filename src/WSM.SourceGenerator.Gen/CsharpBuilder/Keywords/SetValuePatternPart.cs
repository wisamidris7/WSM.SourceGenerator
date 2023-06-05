namespace SourceGenerator.CsharpBuilder.Keywords;
public class SetValuePatternPart : ICSBuilderPart
{
    public SetValuePatternPart(string field, string value)
    {
        Field = field;
        Value = value;
    }

    public string Field { get; }
    public string Value { get; }

    public StringBuilder Build(StringBuilder builder)
    {
        builder.AppendLine($"\t\t{Field} = {Value};");
        return builder;
    }
}
