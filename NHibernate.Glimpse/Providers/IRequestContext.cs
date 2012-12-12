using System.Collections;

namespace NHibernate.Glimpse.Providers
{
    public interface IRequestContext
    {
        IDictionary GetRequestContext();
    }
}