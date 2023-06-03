using System;
using System.Collections.Generic;
using System.Text;

namespace WSM.SourceGenerator.Gen.CsharpBuilder.Keywords
{
    public class NamespacePatternPart : ICSBuilderPart
    {
        public NamespacePatternPart(string name, ICSBuilder body = null)
        {
            Name = name;
            Body = body;
        }

        public string Name { get; }
        public ICSBuilder Body { get; }
        public StringBuilder Build(StringBuilder builder)
        {
            if (Body == null)
            {
                builder.AppendLine($"namespace {Name};");
                builder.AppendLine();
            }
            else
            {
                builder.AppendLine($"namespace {Name}");
                builder.AppendLine("{");
                builder.AppendLine(Body.Build().ToString());
                builder.AppendLine("}");
            }
            return builder;
        }
    }
}
