using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSM.SourceGenerator.Gen.CsharpBuilder;
using WSM.SourceGenerator.Gen.CsharpBuilder.Keywords;
using WSM.SourceGenerator.Gen.Shared;

namespace WSM.SourceGenerator.Gen
{
    [Generator]
    public class AutoDependencyInjection : BaseSourceGenerator
    {
        public override void Execute(GeneratorExecutionContext context)
        {
            base.Execute(context);
            if (Config.Api == null) return;

            var classesNeedAuto = context.Compilation.SyntaxTrees
                    .SelectMany(e => e.GetRoot().DescendantNodes())
                    .Where(e => e.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration))
                    .Where(e => ((ClassDeclarationSyntax)e).AttributeLists.SelectMany(e => e.Attributes).Any(e => e.Name.ToString().StartsWith(Config.Api.StartAutoInjectionAttribute, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            var scopedClasses = classesNeedAuto.Where(e => ((ClassDeclarationSyntax)e).AttributeLists.SelectMany(e => e.Attributes).Any(e => e.Name.ToString().Equals(Config.Api.AutoScopedAttribute, StringComparison.OrdinalIgnoreCase)));
            var singletonClasses = classesNeedAuto.Where(e => ((ClassDeclarationSyntax)e).AttributeLists.SelectMany(e => e.Attributes).Any(e => e.Name.ToString().Equals(Config.Api.AutoSingletonAttribute, StringComparison.OrdinalIgnoreCase)));
            var transientClasses = classesNeedAuto.Where(e => ((ClassDeclarationSyntax)e).AttributeLists.SelectMany(e => e.Attributes).Any(e => e.Name.ToString().Equals(Config.Api.AutoTransientAttribute, StringComparison.OrdinalIgnoreCase)));

            ICSBuilder services = new CSBuilder();

            foreach (var item in singletonClasses)
            {
                services.AddPattern(new DynamicPatternPart($"services.AddSingleton(typeof({item.ToFullString()}));\n"));
            }
            foreach (var item in scopedClasses)
            {
                services.AddPattern(new DynamicPatternPart($"services.AddScoped(typeof({item.ToFullString()}));\n"));
            }
            foreach (var item in transientClasses)
            {
                services.AddPattern(new DynamicPatternPart($"services.AddTransient(typeof({item.ToFullString()}));\n"));
            }
            var servicesInjected = new CSBuilder()
            {
                new UsingPatternPart("Microsoft.Extensions.DependencyInjection"),
                new NamespacePatternPart(Config.Namespace),
                new ClassPatternPart(new CSBuilder()
                {
                    new MethodPatternPart("Add", services, new CSBuilder()
                    {
                        new ParameterPatternPart("services", "IServiceCollection")
                    })
                }, "AppServices")
            };
            var text = SourceText.From(servicesInjected.Build().ToString(), (Encoding)Encoding.UTF32.Clone());
            //context.AddSource("Injections.g.cs", text);
        }
        public override void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
        }
    }
}
