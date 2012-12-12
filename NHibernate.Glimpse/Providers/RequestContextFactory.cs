using System;
using System.Configuration;
using System.Linq;

namespace NHibernate.Glimpse.Providers
{
    public class RequestContextFactory
    {
        public IRequestContext GetRequestContextProvider()
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains("NHibernate.Glimpse.ContextProvider"))
            {
                return new HttpContextProvider();
            }
            var reader = new AppSettingsReader();
            var typeString = reader.GetValue("NHibernate.Glimpse.ContextProvider", typeof(string));
            if (typeString == null || typeString.ToString().Trim() == string.Empty)
            {
                return new HttpContextProvider();
            }
            var parts = typeString.ToString().Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Count() != 2)
            {
                throw new TypeLoadException("IRequestContext type could not be loaded.  The type format should be \"MyAssembly.MyType, MyAssembly\".");
            }
            var o = Activator.CreateInstance(parts[1], parts[0]).Unwrap();
            if (o as IRequestContext == null)
            {
                throw new TypeLoadException("The specified type must implement IRequestContext.");
            }
            return (IRequestContext)o;
        }
    }
}