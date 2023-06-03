using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WSM.SourceGenerator.Gen.Shared;

namespace WSM.SourceGenerator.Gen
{

    [Generator]
    public class ClientServiceGenerator : BaseSourceGenerator, ISourceGenerator
    {
        public override void Execute(GeneratorExecutionContext context)
        {
            base.Execute(context);
            var path = Path.Combine("C:\\Users\\Administrator\\source\\repos\\WSM.SourceGenerator\\samples\\WSM.SourceGenerator.Demo.Client", Path.GetDirectoryName(Config.Client.ControllersPath));
            var controllers = ManualLoad(path).ToList();
            foreach (var item in controllers)
            {
                var root = item.GetRoot().DescendantNodesAndSelf().OfType<MethodDeclarationSyntax>().ToList().First().AttributeLists.SelectMany(e => e.Attributes).First();
            }
        }
        public override void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
        }
        private List<SyntaxTree> ManualLoad(string rootDir)
        {
            List<SyntaxTree> tree = new();
            foreach (var filepath in Directory.GetFiles(rootDir, "*.cs", SearchOption.AllDirectories))
            {
                var file = File.ReadAllText(filepath);
                tree.Add(CSharpSyntaxTree.ParseText(file));
            }
            return tree;
        }
        public static string PathCombine(string basePath, params string[] additional)
        {
            var splits = additional.Select(s => s.Split(pathSplitCharacters)).ToArray();
            var totalLength = splits.Sum(arr => arr.Length);
            var segments = new string[totalLength + 1];
            segments[0] = basePath;
            var i = 0;
            foreach (var split in splits)
            {
                foreach (var value in split)
                {
                    i++;
                    segments[i] = value;
                }
            }
            return Path.Combine(segments);
        }

        static char[] pathSplitCharacters = new char[] { '/', '\\' };
    }
}
