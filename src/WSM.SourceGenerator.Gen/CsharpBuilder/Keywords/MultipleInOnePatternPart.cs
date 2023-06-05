﻿namespace SourceGenerator.CsharpBuilder.Keywords;
public class MultipleInOnePatternPart : ICSBuilderPart
{
    public ICSBuilder Body { get; }

    public MultipleInOnePatternPart(ICSBuilder body)
    {
        Body = body;
    }

    public StringBuilder Build(StringBuilder builder)
    {
        builder.Append(Body.Build().ToString());
        return builder;
    }
}
