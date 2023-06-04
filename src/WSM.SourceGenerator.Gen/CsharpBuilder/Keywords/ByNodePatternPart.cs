using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace WSM.SourceGenerator.Gen.CsharpBuilder.Keywords
{
    public class ByNodePatternPart : ICSBuilderPart
    {
        public ByNodePatternPart(params SyntaxNode[] nodes)
        {
            Nodes = nodes;
        }

        public SyntaxNode[] Nodes { get; }

        public StringBuilder Build(StringBuilder builder)
        {
            foreach (var item in Nodes)
            {
                builder.Append(item.ToString() + "\t");
            }
            return builder;
        }
    }
}
