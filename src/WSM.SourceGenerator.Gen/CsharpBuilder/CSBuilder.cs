namespace SourceGenerator.CsharpBuilder;
public class CSBuilder : List<ICSBuilderPart>, ICSBuilder
{
    public List<object> args;
    public ICSBuilder AddPattern(ICSBuilderPart part, bool conditional = true)
    {
        if (conditional)
            Add(part);
        return this;
    }
    public static StringBuilder Build(List<ICSBuilderPart> patterns)
    {
        var builder = new StringBuilder();
        foreach (var item in patterns)
        {
            item.Build(builder);
        }
        return builder;
    }
    public StringBuilder Build()
    {
        return Build(this);
    }
}
