using System.Collections;
using System.Web;

namespace NHibernate.Glimpse.Providers
{
    public class HttpContextProvider : IRequestContext
    {
        public IDictionary GetRequestContext()
        {
            return HttpContext.Current == null ? null : HttpContext.Current.Items;
        }
    }
}