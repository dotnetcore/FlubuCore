using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore
{
    public class DictionaryWithDefault<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private TValue _default;

        public DictionaryWithDefault()
        {
        }

        public DictionaryWithDefault(TValue defaultValue)
        {
            _default = defaultValue;
        }

        public TValue DefaultValue
        {
            get { return _default; }
            set { _default = value; }
        }

        public new TValue this[TKey key]
        {
            get
            {
                TValue t;
                return base.TryGetValue(key, out t) ? t : _default;
            }

            set { base[key] = value; }
        }
    }
}
