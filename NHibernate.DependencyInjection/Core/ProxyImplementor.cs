using System.Reflection;
using System.Reflection.Emit;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.DependencyInjection.Core
{
	internal class ProxyImplementor
	{
		private const MethodAttributes InterceptorMethodsAttributes = 
			MethodAttributes.Public | 
			MethodAttributes.HideBySig |
			MethodAttributes.SpecialName | 
			MethodAttributes.NewSlot |
			MethodAttributes.Virtual;

		public FieldBuilder InterceptorField { get; private set; }

		public void ImplementProxy(TypeBuilder typeBuilder)
		{
			typeBuilder.AddInterfaceImplementation(typeof (IProxy));
			InterceptorField = typeBuilder
				.DefineField("__interceptor",
							 typeof (Proxy.DynamicProxy.IInterceptor),
							 FieldAttributes.Private);
			var getterMethod = typeBuilder
				.DefineMethod("get_Interceptor",
							  InterceptorMethodsAttributes,
							  CallingConventions.HasThis,
							  typeof (Proxy.DynamicProxy.IInterceptor),
							  new System.Type[0]);
			getterMethod.SetImplementationFlags(MethodImplAttributes.IL);
			var il = getterMethod.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldfld, InterceptorField);
			il.Emit(OpCodes.Ret);
			var setterMethod = typeBuilder
				.DefineMethod("set_Interceptor",
							  InterceptorMethodsAttributes,
							  CallingConventions.HasThis,
							  typeof (void),
							  new[] {typeof (Proxy.DynamicProxy.IInterceptor)});
			setterMethod.SetImplementationFlags(MethodImplAttributes.IL);
			il = setterMethod.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Stfld, InterceptorField);
			il.Emit(OpCodes.Ret);
			var originalSetter = typeof (IProxy).GetMethod("set_Interceptor");
			var originalGetter = typeof (IProxy).GetMethod("get_Interceptor");
			typeBuilder.DefineMethodOverride(setterMethod, originalSetter);
			typeBuilder.DefineMethodOverride(getterMethod, originalGetter);
		}
	}
}