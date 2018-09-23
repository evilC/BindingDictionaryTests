using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BindingDictionaryTestTwo
{
    public class SubscriptionDictionary<TKey, TValue, TEmptyEventArgs>
    {
        public delegate void EmptyHandler(TEmptyEventArgs emptyEventArgs);
        private readonly EmptyHandler _emptyHandler;
        private readonly TEmptyEventArgs _emptyEventArgs;

        public TValue this[TKey key] => Dictionary[key];

        protected readonly ConcurrentDictionary<TKey, TValue> Dictionary = new ConcurrentDictionary<TKey, TValue>();

        public SubscriptionDictionary(TEmptyEventArgs emptyEventArgs, EmptyHandler emptyHandler)
        {
            _emptyEventArgs = emptyEventArgs;
            _emptyHandler = emptyHandler;
        }

        public int Count()
        {
            return Dictionary.Count;
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public IEnumerable<TKey> GetKeys()
        {
            return Dictionary.Keys;
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            return Dictionary.GetOrAdd(key, value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return Dictionary.TryAdd(key, value);
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            if (!Dictionary.TryRemove(key, out value)) return false;
            if (Dictionary.IsEmpty)
            {
                _emptyHandler?.Invoke(_emptyEventArgs);
            }

            return true;

        }
    }
}
