using System;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Proxy;

namespace NHibernate.DependencyInjection.Core
{
    public class DefaultProxyFactory : AbstractProxyFactory
    {
        private readonly ProxyFactory _factory = new ProxyFactory();
        protected static readonly IInternalLogger Log = LoggerProvider.LoggerFor(typeof(DefaultProxyFactory));

        public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            try
            {
                var initializer = new DefaultLazyInitializer(EntityName,
                                                             PersistentClass,
                                                             id,
                                                             GetIdentifierMethod,
                                                             SetIdentifierMethod,
                                                             ComponentIdType,
                                                             session);
                var proxyInstance = IsClassProxy
                                           ? _factory.CreateProxy(PersistentClass, initializer, Interfaces)
                                           : _factory.CreateProxy(Interfaces[0], initializer, Interfaces);
                return (INHibernateProxy)proxyInstance;
            }
            catch (Exception ex)
            {
                Log.Error("Creating a proxy instance failed", ex);
                throw new HibernateException("Creating a proxy instance failed", ex);
            }
        }

        public override object GetFieldInterceptionProxy(object instanceToWrap)
        {
            var interceptor = new DefaultDynamicLazyFieldInterceptor(instanceToWrap);
            return _factory.CreateProxy(PersistentClass, interceptor, new[] { typeof(IFieldInterceptorAccessor) });
        }
    }
}