using Unity.Builder;
using Unity.Builder.Operation;
using Unity.Builder.Strategy;
using Unity.Extension;

namespace UnityExtras.DefaultValueResolver
{
    public class DefaultParameterValueExtension : UnityContainerExtension
    {
        protected override void Initialize() =>
            Context.Strategies.Add(new DefaultValueResolverStrategy(), UnityBuildStage.PreCreation);

        private sealed class DefaultValueResolverStrategy : BuilderStrategy
        {
            public override void PreBuildUp(IBuilderContext context)
            {
                if (!context.BuildComplete)
                {
                    var parameter = context.ParentContext?.CurrentOperation is ConstructorArgumentResolveOperation operation
                        ? context.ParentContext.GetConstructorParameter(operation.ParameterName)
                        : context.ParentContext?.CurrentOperation is MethodArgumentResolveOperation methodOperation
                            ? context.ParentContext.GetMethodParameter(methodOperation.ParameterName)
                            : null;

                    if ((parameter?.IsOptional ?? false) && parameter.HasDefaultValue)
                    {
                        context.Existing = parameter.DefaultValue;
                        context.BuildComplete = true;
                    }
                }

                base.PreBuildUp(context);
            }
        }
    }
}
