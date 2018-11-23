using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Spring.Context;
using Spring.Extensions.DependencyInjection.Internal;

namespace Spring.Extensions.DependencyInjection
{
    /// <summary>
    /// Options for <see cref="SpringServiceProviderFactory"/>
    /// </summary>
    public class SpringServiceProviderOptions
    {
        internal static readonly SpringServiceProviderOptions Default = new SpringServiceProviderOptions();

        /// <summary>
        /// parent Spring.NET ApplicationContext
        /// </summary>
        public IApplicationContext Parent { get; set; }

        /// <summary>
        /// The delegate function for the service type instantiation
        /// </summary>
        public Func<IServiceProvider, Type, object> InstanceActivator { get; set; } = (provider, serviceType) =>
          {
              try
              {
                  return ActivatorHelper.TryCreateInstance(provider, serviceType, out var instance)
                  ? instance
                  : ActivatorUtilities.CreateInstance(provider, serviceType);
              }
              catch (Exception e) when (e is InvalidOperationException || e is TargetInvocationException)
              {
                  var context = provider.GetRequiredService<IApplicationContext>();
                  var factory = ((IConfigurableApplicationContext)context).ObjectFactory;
                  return factory.Autowire(serviceType, Objects.Factory.Config.AutoWiringMode.AutoDetect, true);
              }
          };
    }
}
