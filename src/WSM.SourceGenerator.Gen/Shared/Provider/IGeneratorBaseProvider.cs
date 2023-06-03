using System;
using System.Collections.Generic;
using System.Text;

namespace WSM.SourceGenerator.Gen.Shared.Provider
{
    public interface IGeneratorBaseProvider : IEnumerable<KeyValuePair<Type, (Type impType, Func<bool> condFunc)>>
    {
        public T Get<T>(params object[] args);
        public object? Get(Type type, params object[] args);
    }
}
