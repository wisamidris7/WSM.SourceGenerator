using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSM.SourceGenerator.Gen.CsharpBuilder.Keywords
{
    public class AttributesPatternPart : ICSBuilderPart
    {
        public AttributesPatternPart(string name, ICSBuilder? @params = null)
        {
            Name = name;
            Params = @params;
        }

        public ICSBuilder? Params { get; }
        public string Name { get; }
        public StringBuilder Build(StringBuilder builder)
        {
            builder.Append($"[{Name}");
            if (Params != null )
            {
                builder.Append("(");
                var @params = Params.Select(e => e.Build(new()).ToString());
                builder.Append(string.Join(" ,", @params));
                builder.Append(")");
            }
            builder.AppendLine("]");
            return builder;
        }
    }
}
