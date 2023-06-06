namespace SourceGenerator;
[Generator]
public class AutoDependencyInjection : BaseSourceGenerator
{
    public override void Execute(GeneratorExecutionContext context)
    {
        base.Execute(context);
        if (Config.Injection == null) return;

        var classesNeedAuto = ManualLoad(context)
                .SelectMany(e => e.GetRoot().DescendantNodes())
                .Where(e => e.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration))
                .Where(e => ((ClassDeclarationSyntax)e).AttributeLists.SelectMany(e => e.Attributes).Any(e => e.Name.ToString().StartsWith(Config.Injection.StartAutoInjectionAttribute, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        var scopedClasses = classesNeedAuto.Where(e => ((ClassDeclarationSyntax)e).AttributeLists.SelectMany(e => e.Attributes).Any(e => e.Name.ToString().Equals(Config.Injection.AutoScopedAttribute, StringComparison.OrdinalIgnoreCase)));
        var singletonClasses = classesNeedAuto.Where(e => ((ClassDeclarationSyntax)e).AttributeLists.SelectMany(e => e.Attributes).Any(e => e.Name.ToString().Equals(Config.Injection.AutoSingletonAttribute, StringComparison.OrdinalIgnoreCase)));
        var transientClasses = classesNeedAuto.Where(e => ((ClassDeclarationSyntax)e).AttributeLists.SelectMany(e => e.Attributes).Any(e => e.Name.ToString().Equals(Config.Injection.AutoTransientAttribute, StringComparison.OrdinalIgnoreCase)));

        ICSBuilder services = new CSBuilder();

        foreach (var item in singletonClasses)
        {
            services.AddPattern(new DynamicPatternPart($"services.AddSingleton(typeof({item.ToString()}));\n"));
        }
        foreach (var item in scopedClasses)
        {
            services.AddPattern(new DynamicPatternPart($"services.AddScoped(typeof({item.ToString()}));\n"));
        }
        foreach (var item in transientClasses)
        {
            services.AddPattern(new DynamicPatternPart($"services.AddTransient(typeof({item.ToString()}));\n"));
        }
        var servicesInjected = new CSBuilder()
            {
                new UsingPatternPart("Microsoft.Extensions.DependencyInjection"),
                new NamespacePatternPart(Config.Namespace),
                new ClassPatternPart(new CSBuilder()
                {
                    new MethodPatternPart("AddSourceGeneratorServices", services, new CSBuilder()
                    {
                        new ParameterPatternPart("services", "this IServiceCollection")
                    }, @static: true)
                }, "AppServices", @static: true)
            };
        var text = SourceText.From(servicesInjected.Build().ToString(), (Encoding)Encoding.UTF32.Clone());
        context.AddSource("Injections.g.cs", text);
    }
    public override void Initialize(GeneratorInitializationContext context)
    {
        //Debugger.Launch();
    }
}
