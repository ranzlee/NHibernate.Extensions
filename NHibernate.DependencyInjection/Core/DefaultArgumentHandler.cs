using System;
using System.Reflection;
using System.Reflection.Emit;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.DependencyInjection.Core
{
    internal class DefaultArgumentHandler : IArgumentHandler
    {
        public void PushArguments(ParameterInfo[] methodParameters, ILGenerator il, bool isStatic)
        {
            var parameters = methodParameters ?? new ParameterInfo[0];
            var parameterCount = parameters.Length;
            il.Emit(OpCodes.Ldc_I4, parameterCount);
            il.Emit(OpCodes.Newarr, typeof (object));
            il.Emit(OpCodes.Stloc_S, 0);
            if (parameterCount == 0)
            {
                il.Emit(OpCodes.Ldloc_S, 0);
                return;
            }
            var index = 0;
            var argumentPosition = 1;
            foreach (var param in parameters)
            {
                var parameterType = param.ParameterType.IsByRef ? param.ParameterType.GetElementType() : param.ParameterType;
                il.Emit(OpCodes.Ldloc_S, 0);
                il.Emit(OpCodes.Ldc_I4, index);
                if (param.IsOut)
                {
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Stelem_Ref);
                    argumentPosition++;
                    index++;
                    continue;
                }
                il.Emit(OpCodes.Ldarg, argumentPosition);
                if (param.ParameterType.IsByRef)
                {
                    OpCode ldindInstruction;
                    if(!OpCodesMap.TryGetLdindOpCode(param.ParameterType.GetElementType(), out ldindInstruction))
                    {
                        ldindInstruction = OpCodes.Ldind_Ref;
                    }
                    il.Emit(ldindInstruction);
                }
                if (parameterType == null)
                {
                    throw new InvalidOperationException("parameterType cannot be null");
                }
                if (parameterType.IsValueType || param.ParameterType.IsByRef || parameterType.IsGenericParameter)
                {
                    il.Emit(OpCodes.Box, parameterType);
                }
                il.Emit(OpCodes.Stelem_Ref);
                index++;
                argumentPosition++;
            }
            il.Emit(OpCodes.Ldloc_S, 0);
        }
    }
}