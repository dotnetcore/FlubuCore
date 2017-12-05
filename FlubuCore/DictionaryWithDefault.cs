using System.Collections.Generic;

namespace FlubuCore
{
    public class DictionaryWithDefault<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public DictionaryWithDefault()
        {
        }

        public DictionaryWithDefault(TValue defaultValue)
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