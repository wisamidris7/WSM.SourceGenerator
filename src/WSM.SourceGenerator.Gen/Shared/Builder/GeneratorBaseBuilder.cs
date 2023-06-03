using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSM.SourceGenerator.Gen.Shared.Provider;

namespace WSM.SourceGenerator.Gen.Shared.Builder
{
    public class GeneratorBaseBuilder : IGeneratorBaseBuilder
    {
        private List<Func<(Type bas, Type imp, Func<bool>? cond)>> _builderItems;
        public IGeneratorBaseBuilder Add<TBase, T>(Func<bool> conditional = null)
        {
            return Add(typeof(TBase), typeof(T), conditional);
        }

        public IGeneratorBaseBuilder Add(Type baseType, Type implementType, Func<bool> conditional = null)
        {
            _builderItems ??= new();
            _builderItems.Add(() =>
            {
                return (baseType, implementType, conditional);
            });
            return this;
        }

        public IGeneratorBaseProvider Build()
        {
            return new GeneratorBaseProvider(_builderItems.Select(e =>
            {
                var val = e.Invoke();
                return new KeyValuePair<Type, (Type impType, Func<bool> condFunc)>(val.bas, (val.imp, val.cond));
            }).ToDictionary(e => e.Key, e => e.Value));
        }
    }
}
