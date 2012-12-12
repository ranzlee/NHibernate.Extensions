namespace NHibernate.DependencyInjection.Core
{
    internal class DynProxyTypeValidator : Proxy.DynProxyTypeValidator
    {
        protected override bool HasVisibleDefaultConstructor(System.Type type)
        {
            var constructorParms = BytecodeProvider.EntityInjector.GetConstructorParameters(type);
            if (constructorParms == null || constructorParms.Length == 0)
            {
                return base.HasVisibleDefaultConstructor(type);
            }
            return true;
        }
    }
}