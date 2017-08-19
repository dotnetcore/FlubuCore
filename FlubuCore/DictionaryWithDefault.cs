using System;
using System.Collections.Generic;
using System.Text;

namespace FlubuCore
{
	public class DictionaryWithDefault<TKey, TValue> : Dictionary<TKey, TValue>
	{
		TValue _default;

		public TValue DefaultValue
		{
			get { return _default; }
			set { _default = value; }
		}

		public DictionaryWithDefault() : base() { }

		public DictionaryWithDefault(TValue defaultValue) : base()
		{
			_default = defaultValue;
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
