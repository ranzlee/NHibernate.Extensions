using System.Collections;

namespace NHibernate.Glimpse.Extensibility
{
    public interface IContextProvider
    {
        IDictionary GetContext();
    }
}