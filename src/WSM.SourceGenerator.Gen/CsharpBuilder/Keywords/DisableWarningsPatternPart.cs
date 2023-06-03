using System;
using System.Collections.Generic;
using System.Text;

namespace WSM.SourceGenerator.Gen.CsharpBuilder.Keywords
{
    public class DisableWarningsPatternPart : ICSBuilderPart
    {
        public StringBuilder Build(StringBuilder builder)
        {
            builder.AppendLine("#nullable enable");
            builder.AppendLine("#pragma warning disable 108 ");
            builder.AppendLine("#pragma warning disable 114 ");
            builder.AppendLine("#pragma warning disable 472 ");
            builder.AppendLine("#pragma warning disable 1573");
            builder.AppendLine("#pragma warning disable 1591");
            builder.AppendLine("#pragma warning disable 8073");
            builder.AppendLine("#pragma warning disable 3016");
            builder.AppendLine("#pragma warning disable 8600");
            builder.AppendLine("#pragma warning disable 8603");
            builder.AppendLine("#pragma warning disable 8618");
            builder.AppendLine("#pragma warning disable 8604");
            builder.AppendLine();
            return builder;
        }
    }
}
