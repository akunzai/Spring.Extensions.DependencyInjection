using System;
using Spring.Context;
using Spring.Objects.Factory;

namespace Spring.Extensions.DependencyInjection.Internal
{
    internal static class ApplicationContextExtensions
    {
        public static IApplicationContext Register(this IApplicationContext context, Type serviceType, Func<object> implementationFactory, string name = null, bool isSingleton = true)
        {
            return context.RegisterSingleton(new DelegateFactoryObject(serviceType, implementationFactory)
            {
                IsSingleton = isSingleton
            }, name);
        }

        public static IApplicationContext RegisterSingleton(this IApplicationContext context, object implementationInstance, string name = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (implementationInstance == null)
            {
                throw new ArgumentNullException(nameof(implementationInstance));
            }
            var factory = ((IConfigurableApplicationContext)context).ObjectFactory;
            factory.RegisterSingleton(context.BuildObjectName(implementationInstance.GetType(), name, implementationInstance), implementationInstance);
            return context;
        }

        public static IApplicationContext RegisterSingleton(this IApplicationContext context, Type serviceType, object implementationInstance, string name = null)
        {
            return context.Register(serviceType, () => implementationInstance, name);
        }

        public static IApplicationContext RegisterSingleton<T>(this IApplicationContext context, object implementationInstance, string name = null)
        {
            return context.RegisterSingleton(typeof(T), implementationInstance, name);
        }

        private static string BuildObjectName(this IApplicationContext context, Type serviceType, string name, object implementationInstance = null)
        {
            if (serviceType != null && typeof(IFactoryObject).IsAssignableFrom(serviceType) && implementationInstance is IFactoryObject factoryObject)
            {
                return context.BuildObjectName(factoryObject.ObjectType, name);
            }
            return !string.IsNullOrWhiteSpace(name) && !context.ContainsObject(name)
                    ? name
                    : serviceType != null && !context.ContainsObject(serviceType.FullName)
                        ? serviceType.FullName
                        : Guid.NewGuid().ToString();
        }

        internal static IServiceProvider GetOrCreateServiceProvider(this IApplicationContext context)
        {
            var matchingServices = context.GetObjectNames<IServiceProvider>();
            return (matchingServices?.Count > 0)
                ? (IServiceProvider)context.GetObject(matchingServices[0])
                : new SpringServiceProvider(context, SpringServiceProviderOptions.Default);
        }
    }
}
