using System;
using System.Collections.Generic;
using System.Linq;

namespace SWPCCBilling2.Infrastructure
{
    public static class ObjectExtensions
    {
        public static object[] NonKeyValues(this object o)
        {
            TypeInfo typeInfo = TypeInfo.Get(o.GetType());

            return typeInfo.Properties
                .Where(p => p != typeInfo.KeyProperty)
                .Select(p => p.GetValue(o))
                .ToArray();
        }

        public static object[] AllValues(this object o)
        {
            TypeInfo typeInfo = TypeInfo.Get(o.GetType());

            return typeInfo.Properties
                .Select(p => p.GetValue(o))
                .ToArray();
        }

		public static object[] AllValuesKeyLast(this object o)
		{
			IList<object> values = new List<object>(NonKeyValues(o));
			values.Add(KeyValue(o));
			return values.ToArray();
		}

        public static object KeyValue(this object o)
        {
            TypeInfo typeInfo = TypeInfo.Get(o.GetType());
            return typeInfo.KeyProperty.GetValue(o);
        }
    }
}

