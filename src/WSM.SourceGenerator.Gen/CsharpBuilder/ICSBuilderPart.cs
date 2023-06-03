using System;
using System.Collections.Generic;
using System.Text;

namespace WSM.SourceGenerator.Gen.CsharpBuilder
{
    public interface ICSBuilderPart
    {
        public StringBuilder Build(StringBuilder builder);
    }
}
