using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace WSM.SourceGenerator.Gen.CsharpBuilder.Keywords
{
    public class ReturnValuePatternPart : ICSBuilderPart
    {
        public ReturnValuePatternPart(string value, bool oneLine=false)
        {
            Value = value;
            OneLine = oneLine;
        }
        public ReturnValuePatternPart(ICSBuilder operation, bool oneLine = false)
        {
            Value = operation.Build().ToString();
            OneLine = oneLine;
        }

        public string Value { get; }
        public bool OneLine { get; }

        public StringBuilder Build(StringBuilder builder)
        {
            if (OneLine) return builder.Append($" => {Value}");

            builder.Append($"return {Value};");
            return builder;
        }
    }
}
