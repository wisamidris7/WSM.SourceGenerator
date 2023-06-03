using System;
using System.Collections.Generic;
using System.Text;
using WSM.SourceGenerator.Gen.Shared.Provider;

namespace WSM.SourceGenerator.Gen.Shared.Builder
{
    public interface IGeneratorBaseBuilder
    {
        public IGeneratorBaseBuilder Add<TBase, T>(Func<bool> conditional = null);
        public IGeneratorBaseBuilder Add(Type baseType, Type implementType, Func<bool> conditional = null);
        public IGeneratorBaseProvider Build();
    }
}
