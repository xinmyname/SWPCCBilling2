using System;
using Mono.Data.Sqlite;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace SWPCCBilling2.Infrastructure
{
    public class TypeInfo
    {
        private static Dictionary<Type,TypeInfo> _typeInfoCache;

        public string Name { get; private set; }
        public IList<PropertyDescriptor> Properties { get; private set; }
        public PropertyDescriptor KeyProperty { get; private set; }

        static TypeInfo()
        {
            _typeInfoCache = new Dictionary<Type, TypeInfo>();
        }

        public TypeInfo(Type t)
        {
            Name = t.Name;
            Properties = TypeDescriptor.GetProperties(t).Cast<PropertyDescriptor>().ToList();
            KeyProperty = Properties.FirstOrDefault(p => p.Attributes.OfType<KeyAttribute>().Any());
        }

        public static TypeInfo Get(Type type)
        {
            if (_typeInfoCache.ContainsKey(type))
                return _typeInfoCache[type];

            var typeInfo = new TypeInfo(type);

            _typeInfoCache[type] = typeInfo;

            return typeInfo;
        }

        public static TypeInfo Get<T>()
        {
            return Get(typeof(T));
        }
    }
}