using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.DependencyInjection.Core
{
    internal class DefaultMethodEmitter : IMethodBodyEmitter
    {
        private static readonly MethodInfo GetInterceptor;

        private static readonly MethodInfo GetGenericMethodFromHandle =
            typeof (MethodBase)
                .GetMethod("GetMethodFromHandle",
                           BindingFlags.Public | BindingFlags.Static,
                           null,
                           new[] {typeof (RuntimeMethodHandle), typeof (RuntimeTypeHandle)},
                           null);

        private static readonly MethodInfo GetMethodFromHandle =
            typeof (MethodBase)
                .GetMethod("GetMethodFromHandle",
                           new[] {typeof (RuntimeMethodHandle)});

        private static readonly MethodInfo GetTypeFromHandle =
            typeof (System.Type)
                .GetMethod("GetTypeFromHandle");

        private static readonly MethodInfo HandlerMethod =
            typeof (Proxy.DynamicProxy.IInterceptor)
                .GetMethod("Intercept");
        
        private static readonly ConstructorInfo InfoConstructor;

        private static readonly PropertyInfo InterceptorProperty =
            typeof (IProxy)
                .GetProperty("Interceptor");

        private static readonly ConstructorInfo NotImplementedConstructor =
            typeof (NotImplementedException)
                .GetConstructor(new System.Type[0]);

        private readonly IArgumentHandler _argumentHandler;

        static DefaultMethodEmitter()
        {
            GetInterceptor = InterceptorProperty.GetGetMethod();
            var constructorTypes = new[]
                                       {
                                           typeof (object),
                                           typeof (MethodInfo),
                                           typeof (StackTrace),
                                           typeof (System.Type[]),
                                           typeof (object[])
                                       };
            InfoConstructor = typeof (InvocationInfo).GetConstructor(constructorTypes);
        }

        public DefaultMethodEmitter() : this(new DefaultArgumentHandler()) {}

        public DefaultMethodEmitter(IArgumentHandler argumentHandler)
        {
            _argumentHandler = argumentHandler;
        }

        public void EmitMethodBody(ILGenerator il, MethodInfo method, FieldInfo field)
        {
            var parameters = method.GetParameters();
            il.DeclareLocal(typeof (object[]));
            il.DeclareLocal(typeof (InvocationInfo));
            il.DeclareLocal(typeof(System.Type[]));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, GetInterceptor);
            var skipThrow = il.DefineLabel();
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Bne_Un, skipThrow);
            il.Emit(OpCodes.Newobj, NotImplementedConstructor);
            il.Emit(OpCodes.Throw);
            il.MarkLabel(skipThrow);
            il.Emit(OpCodes.Ldarg_0);
            var declaringType = method.DeclaringType;
            il.Emit(OpCodes.Ldtoken, method);
            if (declaringType != null && declaringType.IsGenericType)
            {
                il.Emit(OpCodes.Ldtoken, declaringType);
                il.Emit(OpCodes.Call, GetGenericMethodFromHandle);
            }
            else
            {
                il.Emit(OpCodes.Call, GetMethodFromHandle);
            }
            il.Emit(OpCodes.Castclass, typeof (MethodInfo));
            PushStackTrace(il);
            PushGenericArguments(method, il);
            _argumentHandler.PushArguments(parameters, il, false);
            il.Emit(OpCodes.Newobj, InfoConstructor);
            il.Emit(OpCodes.Stloc_1);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Callvirt, HandlerMethod);
            PackageReturnType(method, il);
            SaveRefArguments(il, parameters);
            il.Emit(OpCodes.Ret);
        }

        private static void SaveRefArguments(ILGenerator il, IEnumerable<ParameterInfo> parameters)
        {
            var getArguments = typeof (InvocationInfo).GetMethod("get_Arguments");
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Call, getArguments);
            il.Emit(OpCodes.Stloc_0);
            foreach (var param in parameters)
            {
                var typeName = param.ParameterType.Name;
                var isRef = param.ParameterType.IsByRef && typeName.EndsWith("&");
                if (!isRef) continue;
                il.Emit(OpCodes.Ldarg, param.Position + 1);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldc_I4, param.Position);
                il.Emit(OpCodes.Ldelem_Ref);
                var unboxedType = param.ParameterType.GetElementType();
                if (unboxedType == null)
                {
                    throw new InvalidOperationException("unboxedType cannot be null");
                }
                il.Emit(OpCodes.Unbox_Any, unboxedType);
                var stind = GetStindInstruction(param.ParameterType);
                il.Emit(stind);
            }
        }

        private static OpCode GetStindInstruction(System.Type parameterType)
        {
            if (parameterType.IsByRef)
            {
                OpCode stindOpCode;
                if(OpCodesMap.TryGetStindOpCode(parameterType.GetElementType(), out stindOpCode))
                {
                    return stindOpCode;
                }
            }
            return OpCodes.Stind_Ref;
        }

        private static void PushStackTrace(ILGenerator il)
        {
            il.Emit(OpCodes.Ldnull);
        }

        private static void PushGenericArguments(MethodInfo method, ILGenerator il)
        {
            var typeParameters = method.GetGenericArguments();
            var genericTypeCount = typeParameters.Length;
            il.Emit(OpCodes.Ldc_I4, genericTypeCount);
            il.Emit(OpCodes.Newarr, typeof(System.Type));
            if (genericTypeCount == 0) return;
            for (var index = 0; index < genericTypeCount; index++)
            {
                var currentType = typeParameters[index];
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4, index);
                il.Emit(OpCodes.Ldtoken, currentType);
                il.Emit(OpCodes.Call, GetTypeFromHandle);
                il.Emit(OpCodes.Stelem_Ref);
            }
        }

        private static void PackageReturnType(MethodInfo method, ILGenerator il)
        {
            var returnType = method.ReturnType;
            if (returnType == typeof (void))
            {
                il.Emit(OpCodes.Pop);
                return;
            }
            il.Emit(OpCodes.Unbox_Any, returnType);
        }
    }
}