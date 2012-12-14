using System.Collections;
using System.Web;

namespace NHibernate.Glimpse.Extensibility
{
    public class HttpContextProvider : IContextProvider
    {
        public IDictionary GetContext()
        {
            return HttpContext.Current == null ? null : HttpContext.Current.Items;
        }
    }
}