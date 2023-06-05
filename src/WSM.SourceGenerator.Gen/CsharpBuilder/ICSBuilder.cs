namespace SourceGenerator.CsharpBuilder;
public interface ICSBuilder : IList<ICSBuilderPart>
{
    ICSBuilder AddPattern(ICSBuilderPart part, bool conditional = true);
    StringBuilder Build();
}
