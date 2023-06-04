using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSM.SourceGenerator.Gen.CsharpBuilder.Enums;

namespace WSM.SourceGenerator.Gen.CsharpBuilder.Keywords
{
    public class MethodPatternPart : ICSBuilderPart
    {
        public MethodPatternPart(string name, ICSBuilder body, ICSBuilder? param = null, string returnValue="void", ProtectionTypeEnum protectionType = ProtectionTypeEnum.Public)
        {
            Body = body;
            Params = param;
            ReturnValue = returnValue;
            Name = name;
            ProtectionType = protectionType;
        }
        public ICSBuilder Body { get; }
        public ICSBuilder? Params { get; }
        public string ReturnValue { get; }
        public string Name { get; }
        public ProtectionTypeEnum ProtectionType { get; }
        public StringBuilder Build(StringBuilder builder)
        {
            builder.Append(ProtectionType.ToString().ToLower());
            builder.Append($" {ReturnValue} ");
            builder.Append($"{Name}(");
            if (Params != null)
            {
                var @params = Params.Select(e => e.Build(new()).ToString());
                builder.Append(string.Join(" ,", @params));
            }
            builder.Append(")");
            if (Body != null)
            {
                builder.AppendLine("\t\n{");
                builder.AppendLine(Body.Build().ToString());
                builder.AppendLine("\t}");
            }
            else
            {
                builder.Append(" { }\n");
            }
            return builder;
        }
    }
}
