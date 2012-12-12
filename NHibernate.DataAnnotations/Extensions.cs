using System.Collections.Generic;
using NHibernate.DataAnnotations;
using NHibernate.DataAnnotations.Core;

// ReSharper disable CheckNamespace
namespace NHibernate
// ReSharper restore CheckNamespace
{
    public static class Extensions
    {
        public static ISessionValidator GetValidator(this ISession session)
        {
            var interceptor = session.GetSessionImplementation().Interceptor as ValidationInterceptor;
            if (interceptor == null) return null;
            return interceptor.GetSessionAuditor();
        }

        public static EntityPersistenceContext GetEntityPersistenceContext(this IDictionary<object, object> dictionary)
        {
            if (!dictionary.ContainsKey(EntityValidator.ContextKey)) return null;
            return dictionary[EntityValidator.ContextKey] as EntityPersistenceContext;
        }
    }
}