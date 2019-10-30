using System;
using Microsoft.Extensions.DependencyInjection;
using Spring.Context;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection.Internal;

namespace Spring.Extensions.DependencyInjection
{
    public class SpringServiceProviderFactory : IServiceProviderFactory<IApplicationContext>
    {
        public const string GenericTypePrefix = "<";
        internal const string ApplicationContextName = "spring.service.provider";
        private readonly SpringServiceProviderOptions _options;

        public SpringServiceProviderFactory() : this(SpringServiceProviderOptions.Default)
        {
        }

        public SpringServiceProviderFactory(IApplicationContext parent) : this(new SpringServiceProviderOptions { Parent = parent })
        {
        }

        public SpringServiceProviderFactory(SpringServiceProviderOptions options)
        {
            _options = options;
        }

        public SpringServiceProviderFactory(Action<SpringServiceProviderOptions> configureDelegate)
        {
            _options = new SpringServiceProviderOptions();
            configureDelegate?.Invoke(_options);
        }

        public IApplicationContext CreateBuilder(IServiceCollection services)
        {
            var context = new GenericApplicationContext(ApplicationContextName, true, _options.Parent);

            if (services != null)
            {
                foreach (var service in services)
                {
                    if (service.ImplementationType != null)
                    {
                        if (service.ServiceType.IsGenericTypeDefinition && service.ImplementationType.IsGenericTypeDefinition)
                        {
                            // HACK: we leave a clue here to make SpringServiceProvider can binding generic type
                            context.RegisterSingleton(service.ImplementationType, GenericTypePrefix + service.ServiceType.AssemblyQualifiedName);
                            continue;
                        }
                        context.Register(service.ServiceType, () =>
                        {
                            var provider = context.GetOrCreateServiceProvider();
                            return _options.InstanceActivator(provider, service.ImplementationType);
                        }, isSingleton: service.Lifetime != ServiceLifetime.Transient);
                    }
                    else if (service.ImplementationFactory != null)
                    {
                        context.Register(service.ServiceType, () =>
                        {
                            var provider = context.GetOrCreateServiceProvider();
                            return service.ImplementationFactory(provider);
                        }, isSingleton: service.Lifetime != ServiceLifetime.Transient);
                    }
                    else
                    {
                        context.RegisterSingleton(service.ServiceType, service.ImplementationInstance);
                    }
                }
            }

            return context;
        }

        public IServiceProvider CreateServiceProvider(IApplicationContext containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }
            containerBuilder.RegisterSingleton(_options);
            containerBuilder.RegisterSingleton<IApplicationContext>(containerBuilder);
            containerBuilder.RegisterSingleton<IServiceProviderFactory<IApplicationContext>>(this);
            containerBuilder.RegisterSingleton<IServiceScopeFactory>(new SpringServiceScopeFactory(containerBuilder, _options));

            var provider = new SpringServiceProvider(containerBuilder, _options);
            containerBuilder.RegisterSingleton<IServiceProvider>(provider);
            return provider;
        }
    }
}
