using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace UnityExtras.DefaultValueResolver
{
    internal static class Extensions
    {
        public static ParameterInfo GetConstructorParameter(this IBuilderContext context, string parameterName)
        {
            var selector = context.Policies.Get<IConstructorSelectorPolicy>(context.BuildKey,
                out var resolverPolicyDestination);
            var selectedConstructor = selector?.SelectConstructor(context, resolverPolicyDestination);

            return selectedConstructor?.Constructor.GetParameters()
                .SingleOrDefault(p => p.Name == parameterName);
        }

        public static ParameterInfo GetMethodParameter(this IBuilderContext context, string parameterName)
        {
            var selector = context.Policies.Get<IMethodSelectorPolicy>(context.BuildKey,
                out var resolverPolicyDestination);
            var selectedMethod = selector?.SelectMethods(context, resolverPolicyDestination);

            return selectedMethod.FirstOrDefault()?.Method.GetParameters()
                .SingleOrDefault(p => p.Name == parameterName);
        }
    }
}
