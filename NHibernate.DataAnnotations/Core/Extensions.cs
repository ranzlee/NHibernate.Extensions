using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHibernate.DataAnnotations.Core
{
    internal static class Extensions
    {
        private static readonly ConcurrentDictionary<System.Type, IList<PropertyInfo>> DictPropertyCache = new ConcurrentDictionary<System.Type, IList<PropertyInfo>>();

        internal static IEnumerable<PropertyInfo> GetPropertiesFromCache(this System.Type t)
        {
            if (DictPropertyCache.ContainsKey(t)) return DictPropertyCache[t];
            IList<PropertyInfo> pia;
            if (t.IsInterface)
            {
                pia = GetInterfaceProperties(t, new List<PropertyInfo>());
                DictPropertyCache.GetOrAdd(t, pia);    
            }
            else
            {
                pia = t
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .OrderBy(i => i.Name)
                .ToList();
                DictPropertyCache.GetOrAdd(t, pia);    
            }
            return pia;      
        }

        private static List<PropertyInfo> GetInterfaceProperties(System.Type interfaceToReflect, List<PropertyInfo> properties)
        {
            var interfaces = interfaceToReflect.GetInterfaces();
            foreach (var inter in interfaces)
            {
                properties.AddRange(GetInterfaceProperties(inter, properties));
            }
            properties.AddRange(interfaceToReflect.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList());
            return properties.Distinct().OrderBy(i => i.Name).ToList();
        }

        //public static string GetMemberName<T>(this T o, Expression<Func<T, object>> property)
        //{
        //    return ExpressionHelper.GetMemberInfoFromExpression(property).Member.Name;
        //}
    }
}