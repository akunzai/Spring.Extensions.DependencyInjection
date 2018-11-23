using System;
using Spring.Context;
using Spring.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///  Extension methods on <see cref="IServiceCollection"/> to register the <see cref="IServiceProviderFactory{TContainerBuilder}"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the <see cref="SpringServiceProviderFactory"/> to service collection
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSpring(this IServiceCollection services)
        {
            services.AddSingleton<IServiceProviderFactory<IApplicationContext>>(new SpringServiceProviderFactory());
            return services;
        }

        /// <summary>
        /// Add the <see cref="SpringServiceProviderFactory"/> to service collection with parent ApplicationContext
        /// </summary>
        /// <param name="services"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IServiceCollection AddSpring(this IServiceCollection services, IApplicationContext parent)
        {
            services.AddSingleton<IServiceProviderFactory<IApplicationContext>>(new SpringServiceProviderFactory(parent));
            return services;
        }

        /// <summary>
        /// Add the <see cref="SpringServiceProviderFactory"/> to service collection with options
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddSpring(this IServiceCollection services, SpringServiceProviderOptions options)
        {
            services.AddSingleton<IServiceProviderFactory<IApplicationContext>>(new SpringServiceProviderFactory(options));
            return services;
        }

        /// <summary>
        /// Add the <see cref="SpringServiceProviderFactory"/> to service collection with options configure delegate
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureDelegate"></param>
        /// <returns></returns>
        public static IServiceCollection AddSpring(this IServiceCollection services, Action<SpringServiceProviderOptions> configureDelegate)
        {
            services.AddSingleton<IServiceProviderFactory<IApplicationContext>>(new SpringServiceProviderFactory(configureDelegate));
            return services;
        }
    }
}
