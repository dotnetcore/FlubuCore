using System;
using System.Collections.Generic;

namespace FlubuCore.Infrastructure
{
    public class DictionaryWithDefault<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public DictionaryWithDefault()
        {
        }

        public DictionaryWithDefault(IEqualityComparer<TKey> equalityComparer)
            : base(equalityComparer)
        {
        }

        public DictionaryWithDefault(TValue defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public DictionaryWithDefault(TValue defaultValue, IEqualityComparer<TKey> equalityComparer)
            : base(equalityComparer)
        {
            DefaultValue = defaultValue;
        }

        public TValue DefaultValue { get; set; }

        public new TValue this[TKey key]
        {
            get
            {
                TValue t;
                return TryGetValue(key, out t) ? t : DefaultValue;
            }

            set => base[key] = value;
        }
    }
}