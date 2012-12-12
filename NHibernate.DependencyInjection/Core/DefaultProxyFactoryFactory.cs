using NHibernate.Bytecode;
using NHibernate.Proxy;

namespace NHibernate.DependencyInjection.Core
{
    public class DefaultProxyFactoryFactory : IProxyFactoryFactory
    {
        public IProxyFactory BuildProxyFactory()
        {
            return new DefaultProxyFactory();
        }

        public IProxyValidator ProxyValidator
        {
            get { return new DynProxyTypeValidator(); }
        }

        public bool IsInstrumented(System.Type entityClass)
        {
            return true;
        }

        public bool IsProxy(object entity)
        {
            return entity is INHibernateProxy;
        }
    }
}