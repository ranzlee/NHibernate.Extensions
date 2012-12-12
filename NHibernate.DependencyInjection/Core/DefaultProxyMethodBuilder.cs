using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.DependencyInjection.Core
{
	internal class DefaultyProxyMethodBuilder : IProxyMethodBuilder
	{
		public DefaultyProxyMethodBuilder() : this(new DefaultMethodEmitter()) {}

		public DefaultyProxyMethodBuilder(IMethodBodyEmitter emitter)
		{
			if (emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}
			MethodBodyEmitter = emitter;
		}

		public IMethodBodyEmitter MethodBodyEmitter { get; private set; }

		public void CreateProxiedMethod(FieldInfo field, MethodInfo method, TypeBuilder typeBuilder)
		{
			const MethodAttributes methodAttributes = 
				MethodAttributes.Public | 
				MethodAttributes.HideBySig |
				MethodAttributes.Virtual;
			var parameters = method.GetParameters();
			var methodBuilder = typeBuilder
				.DefineMethod(method.Name,
							  methodAttributes,
							  CallingConventions.HasThis,
							  method.ReturnType,
							  parameters.Select(param => param.ParameterType).ToArray());
			var typeArgs = method.GetGenericArguments();
			if (typeArgs.Length > 0)
			{
				var typeNames = new List<string>();
				for (int index = 0; index < typeArgs.Length; index++)
				{
					typeNames.Add(string.Format("T{0}", index));
				}
				var typeArgsBuilder = methodBuilder.DefineGenericParameters(typeNames.ToArray());
				for (int index = 0; index < typeArgs.Length; index++)
				{
					typeArgsBuilder[index].SetInterfaceConstraints(typeArgs[index].GetGenericParameterConstraints());
				}
			}
			var il = methodBuilder.GetILGenerator();
			Debug.Assert(MethodBodyEmitter != null);
			MethodBodyEmitter.EmitMethodBody(il, method, field);
		}
	}
}