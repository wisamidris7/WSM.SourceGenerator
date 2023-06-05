namespace SourceGenerator.CsharpBuilder.Keywords;
public class DynamicPatternPart : ICSBuilderPart
{
    public string Content { get; set; }
    public bool StringType { get; }

    public DynamicPatternPart(string content, bool stringType = false)
    {
        Content = content;
        StringType = stringType;
    }

    public StringBuilder Build(StringBuilder builder)
    {
        if (StringType)
            builder.Append("\"");
        builder.Append(Content);
        if (StringType)
            builder.Append("\"");
        return builder;
    }
}
