using NHibernate.Bytecode;
using NHibernate.Proxy;

namespace NHibernate.DependencyInjection.Core
{
    internal class ProxyFactoryFactory : IProxyFactoryFactory
    {
        private readonly DefaultProxyFactoryFactory _defaultProxyFactoryFactory;

        internal ProxyFactoryFactory()
        {
            _defaultProxyFactoryFactory = new DefaultProxyFactoryFactory();
        }

        public IProxyFactory BuildProxyFactory()
        {
            return _defaultProxyFactoryFactory.BuildProxyFactory();
        }

        public bool IsInstrumented(System.Type entityClass)
        {
            return _defaultProxyFactoryFactory.IsInstrumented(entityClass);
        }

        public bool IsProxy(object entity)
        {
            return _defaultProxyFactoryFactory.IsProxy(entity);
        }

        public IProxyValidator ProxyValidator
        {
            get { return new DynProxyTypeValidator(); }
        }
    }
}