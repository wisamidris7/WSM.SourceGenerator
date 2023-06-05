namespace SourceGenerator.CsharpBuilder.Keywords;
public class UsingPatternPart : ICSBuilderPart
{
    public string? Namespace { get; }
    public bool UsingNamespace { get; }
    public string? Param { get; }
    public ICSBuilder? Body { get; }

    public UsingPatternPart(string @namespace)
    {
        UsingNamespace = true;
        Namespace = @namespace;
    }
    public UsingPatternPart(string param, ICSBuilder body)
    {
        UsingNamespace = false;
        Param = param;
        Body = body;
    }
    public StringBuilder Build(StringBuilder builder)
    {
        if (UsingNamespace && Namespace != null)
        {
            builder.AppendLine($"using {Namespace};");
        }
        else if (Param != null && Body != null)
        {
            builder.AppendLine($"using ({Param})");
            builder.AppendLine("{");
            builder.AppendLine(Body.Build().ToString());
            builder.AppendLine("}");
        }
        return builder;
    }
}
