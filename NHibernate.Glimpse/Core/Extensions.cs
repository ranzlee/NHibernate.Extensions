using System;

using Glimpse.Core.Extensibility;

namespace NHibernate.Glimpse.Core
{
    internal static class Extensions
    {
        internal static string UppercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s) || s.Trim() == string.Empty) return string.Empty;
            var a = s.ToCharArray();
            if (a.Length == 0) return string.Empty;
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        internal static Func<RuntimePolicy> Safe(this Func<RuntimePolicy> runtimePolicyStrategy)
        {
            return () =>
            {
                try
                {
                    return runtimePolicyStrategy();
                }
                catch (NullReferenceException)
                {
                    // This happens when runtimePolicy is requested without HttpContext (for example executing NHibernate query in background thread)
                    return RuntimePolicy.Off;
                }
            };
        }
    }
}