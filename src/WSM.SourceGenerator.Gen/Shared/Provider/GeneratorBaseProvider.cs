using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSM.SourceGenerator.Gen.Shared.Provider
{
    public class GeneratorBaseProvider : IGeneratorBaseProvider
    {
        public Dictionary<Type, (Type impType, Func<bool> condFunc)> Items => _items.Where(e => e.Value.condFunc == null || e.Value.condFunc.Invoke()).ToDictionary(e => e.Key, e => e.Value);
        private Dictionary<Type, (Type impType, Func<bool> condFunc)> _items;

        public GeneratorBaseProvider(Dictionary<Type, (Type impType, Func<bool> condFunc)> items)
        {
            _items = items;
        }

        public T Get<T>(params object[] args)
        {
            var type = typeof(T);
            var instance = Get(type, args);
            if (instance is T t)
                return t;
            throw new InvalidOperationException($"{type.Name} Is Get Null Error May Be From Constructor Or Type Not Existing");
        }

        public object? Get(Type type, params object[] args)
        {
            var activitorInstance = Activator.CreateInstance(Items[type].impType, args);
            if (activitorInstance != null)
                return activitorInstance;
            return default;
        }


        public IEnumerator<KeyValuePair<Type, (Type impType, Func<bool> condFunc)>> GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item;
            }
        }
    }
}
