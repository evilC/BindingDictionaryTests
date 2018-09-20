using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BindingDictionaryTestOne
{
    /// <summary>
    /// Generic wrapper for ConcurrentDictionary that fires a delegate if the dictionary becomes empty
    /// </summary>
    /// <typeparam name="TKey">The type of key used in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of value used in the dictionary</typeparam>
    /// <typeparam name="TEmptyEventArgs">The argument type of the delegate</typeparam>
    public class DictionaryWrapper<TKey, TValue, TEmptyEventArgs> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        public delegate void EmptyHandler(TEmptyEventArgs emptyEventArgs);
        private readonly EmptyHandler _emptyHandler;
        private readonly TEmptyEventArgs _emptyEventArgs;

        public TValue this[TKey key] => Dictionary[key];
        public bool IsEmpty => Dictionary.IsEmpty;
        public int Count => Dictionary.Count;

        protected readonly ConcurrentDictionary<TKey, TValue> Dictionary = new ConcurrentDictionary<TKey, TValue>();

        public DictionaryWrapper(TEmptyEventArgs emptyEventArgs, EmptyHandler emptyHandler)
        {
            _emptyEventArgs = emptyEventArgs;
            _emptyHandler = emptyHandler;
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            return Dictionary.GetOrAdd(key, value);
        }

        public void Add(TKey key, TValue value)
        {
            Dictionary.TryAdd(key, value);
        }

        public void Remove(TKey key)
        {
            if (Dictionary.TryRemove(key, out _))
            {
                if (Dictionary.IsEmpty)
                {
                    _emptyHandler?.Invoke(_emptyEventArgs);
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            return Dictionary.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}