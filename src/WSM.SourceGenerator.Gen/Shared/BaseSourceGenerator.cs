namespace SourceGenerator.Shared;

public abstract class BaseSourceGenerator : ISourceGenerator
{
    protected GenerationConfig Config;
    protected IEnumerable<SyntaxTree> ManualLoad(GeneratorExecutionContext context)
    {
        if (Config.Sources != null)
        {
            foreach (var item in context.Compilation.SyntaxTrees)
            {
                yield return item;
            }
        }
        else
        {
            foreach (var rootDir in Config.Sources)
            {
                foreach (var filepath in Directory.GetFiles(rootDir, "*.cs", SearchOption.AllDirectories))
                {
                    var file = File.ReadAllText(filepath);
                    yield return CSharpSyntaxTree.ParseText(file);
                }
            }
        }
    }

    public virtual void Execute(GeneratorExecutionContext context)
    {
        Config = new GenerationConfigService().GetConfig(context);
    }
    public abstract void Initialize(GeneratorInitializationContext context);
}
