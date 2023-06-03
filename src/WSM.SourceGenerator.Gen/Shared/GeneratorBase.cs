using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace WSM.SourceGenerator.Gen.Shared
{
    public abstract class GeneratorBase
    {
        public abstract void Execute(GeneratorExecutionContext context);
    }
}
