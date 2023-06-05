namespace SourceGenerator.CsharpBuilder.Keywords;
public class DisableWarningsPatternPart : ICSBuilderPart
{
    public StringBuilder Build(StringBuilder builder)
    {
        builder.Append(GeneratorUtilities.DisableStart);
        return builder;
    }
}
