using System;
using System.Collections.Generic;
using System.Text;

namespace WSM.SourceGenerator.Gen.CsharpBuilder
{
    public interface ICSBuilder : IList<ICSBuilderPart>
    {
        ICSBuilder AddPattern(ICSBuilderPart part, bool conditional=true);
        StringBuilder Build();
    }
}
