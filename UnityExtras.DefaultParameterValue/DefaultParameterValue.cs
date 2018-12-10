using System;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Operation;
using Unity.Injection;
using Unity.Policy;

namespace UnityExtras.DefaultValueResolver
{
    public class DefaultParameterValue<T> : InjectionParameterValue
    {
        public override string ParameterTypeName => typeof(T).GetTypeInfo().Name;

        public override IResolverPolicy GetResolverPolicy(Type typeToBuild) =>
            new DefaultValueResolverPolicy();

        public override bool MatchesType(Type t) => t == typeof(T);

        private sealed class DefaultValueResolverPolicy : IResolverPolicy
        {
            public object Resolve(IBuilderContext context)
            {
                if (context.CurrentOperation is ConstructorArgumentResolveOperation operation)
                {
                    var parameter = context.GetConstructorParameter(operation.ParameterName);

                    if (parameter.IsOptional && parameter.HasDefaultValue)
                        return parameter.DefaultValue;
                    else
                        throw new InvalidOperationException($"Parameter {parameter.Name} of a constructor {operation.ConstructorSignature} does not have a default value.");
                }
                else if (context.CurrentOperation is MethodArgumentResolveOperation methodOperation)
                {
                    var parameter = context.GetMethodParameter(methodOperation.ParameterName);

                    if (parameter.IsOptional && parameter.HasDefaultValue)
                        return parameter.DefaultValue;
                    else
                        throw new InvalidOperationException($"Parameter {parameter.Name} of a method {methodOperation.MethodSignature} does not have a default value.");
                }
                else
                    throw new InvalidOperationException($"DefaultParameterValue is only supported for constructor and method injection.");
            }
        }
    }
}
