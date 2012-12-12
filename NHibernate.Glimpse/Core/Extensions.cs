namespace NHibernate.Glimpse.Core
{
    internal static class Extensions
    {
        internal static string UppercaseFirst(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var a = s.ToCharArray();
            if (a.Length == 0) return string.Empty;
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
    }
}