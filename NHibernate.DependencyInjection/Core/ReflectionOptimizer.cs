using System;
using System.Reflection;
using NHibernate.Properties;

namespace NHibernate.DependencyInjection.Core
{
    internal class ReflectionOptimizer : Bytecode.Lightweight.ReflectionOptimizer
    {
        internal ReflectionOptimizer
            (System.Type mappedType, 
             IGetter[] getters, 
             ISetter[] setters) : base(mappedType, getters, setters) { }

        public override object CreateInstance()
        {
            if (ReferenceEquals(mappedType, null)) return base.CreateInstance();
            if (ReferenceEquals(mappedType.FullName, null)) return base.CreateInstance();
            var constructorParms = BytecodeProvider.EntityInjector.GetConstructorParameters(mappedType);
            return (constructorParms == null || constructorParms.Length == 0)
                       ? base.CreateInstance()
                       : Activator.CreateInstance(mappedType,
                                                  BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                                  null, constructorParms, null);
            //: Activator.CreateInstance(mappedType, constructorParms) ;
        }

        protected override void ThrowExceptionForNoDefaultCtor(System.Type type)
        {
            var constructorParms = BytecodeProvider.EntityInjector.GetConstructorParameters(type);
            if (constructorParms != null && constructorParms.Length > 0) return;
            base.ThrowExceptionForNoDefaultCtor(type);
        }
    }
}