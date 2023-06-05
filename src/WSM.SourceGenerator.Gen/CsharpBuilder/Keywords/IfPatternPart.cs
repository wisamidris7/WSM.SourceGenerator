namespace SourceGenerator.CsharpBuilder.Keywords;
public class IfPatternPart : ICSBuilderPart
{
    public IfPatternPart(string statement, ICSBuilder body)
    {
        Statement = statement;
        Body = body;
    }

    public string Statement { get; }
    public ICSBuilder Body { get; }

    public StringBuilder Build(StringBuilder builder)
    {
        if (Body.Count == 1)
            return builder.Append($"if ({Statement}) {Body.Build()}");

        builder.AppendLine($"if ({Statement})");
        builder.AppendLine("{");
        builder.AppendLine(Body.Build().ToString());
        builder.AppendLine("}");

        return builder;
    }
}
