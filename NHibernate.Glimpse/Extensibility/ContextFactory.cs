using System;
using System.Configuration;
using System.Linq;

namespace NHibernate.Glimpse.Extensibility
{
    public class ContextFactory
    {
        public IContextProvider GetContextProvider()
        {
            if (!ConfigurationManager.AppSettings.AllKeys.Contains("NHibernate.Glimpse.Extensibility.IContextProvider"))
            {
                return new HttpContextProvider();
            }
            var reader = new AppSettingsReader();
            var typeString = reader.GetValue("NHibernate.Glimpse.Extensibility.IContextProvider", typeof(string));
            if (typeString == null || typeString.ToString().Trim() == string.Empty)
            {
                return new HttpContextProvider();
            }
            var parts = typeString.ToString().Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Count() != 2)
            {
                throw new TypeLoadException("NHibernate.Glimpse.Extensibility.IContextProvider type could not be loaded.  The type format should be \"MyAssembly.MyType, MyAssembly\".");
            }
            var o = Activator.CreateInstance(parts[1], parts[0]).Unwrap();
            if (o as IContextProvider == null)
            {
                throw new TypeLoadException("The specified type must implement NHibernate.Glimpse.Extensibility.IContextProvider.");
            }
            return (IContextProvider)o;
        }
    }
}