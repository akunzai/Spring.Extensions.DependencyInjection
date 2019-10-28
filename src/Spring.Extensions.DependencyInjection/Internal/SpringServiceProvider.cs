using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Spring.Context;
using Spring.Core.TypeConversion;
using Spring.Objects.Factory;

namespace Spring.Extensions.DependencyInjection.Internal
{
    internal class SpringServiceProvider : IServiceProvider, ISupportRequiredService, IDisposable
    {
        private readonly ConcurrentBag<Type> _nonResolvableTypes = new ConcurrentBag<Type>();
        private readonly IApplicationContext _context;
        private readonly SpringServiceProviderOptions _options;
        private bool _disposed;

        public SpringServiceProvider(IApplicationContext context, SpringServiceProviderOptions options)
        {
            _context = context;
            _options = options;
        }

        public object GetService(Type serviceType)
        {
            return LookupService(_context, serviceType);
        }

        public object GetRequiredService(Type serviceType)
        {
            if (_nonResolvableTypes.Contains(serviceType))
            {
                ThrowRequiredServiceTypeNotFoundException(serviceType);
            }
            var requiredService = LookupService(_context, serviceType);
            if (requiredService == null)
            {
                _nonResolvableTypes.Add(serviceType);
                ThrowRequiredServiceTypeNotFoundException(serviceType);
            }
            return requiredService;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        private static void ThrowRequiredServiceTypeNotFoundException(Type serviceType)
        {
            throw new InvalidOperationException($"could not find service from Spring container with type: {serviceType}");
        }

        private object GetServiceIntetnal(IApplicationContext context, Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider))
            {
                return this;
            }
            // TODO: the same service type in CodeConfigApplicationContext or XmlApplicationContext was preferred for SpringServiceProvider in SpringServiceScope
            var objectNames = context.GetObjectNamesForType(serviceType);
            if (objectNames.Count > 0)
            {
                if (objectNames.Count == 1)
                {
                    return context.GetObject(objectNames[0]);
                }
                // perfect type matched object is preferred
                var factory = ((IConfigurableApplicationContext)context).ObjectFactory;
                var perfectMatchedObjectName = new List<string>();
                foreach (var objectName in objectNames)
                {
                    var definition = factory.GetObjectDefinition(objectName);
                    if (definition != null && definition.ObjectType == serviceType)
                    {
                        perfectMatchedObjectName.Add(objectName);
                    }
                    var singleton = factory.GetSingleton(objectName);
                    if (singleton != null && singleton is IFactoryObject factoryObject && factoryObject.ObjectType == serviceType)
                    {
                        perfectMatchedObjectName.Add(objectName);
                    }
                }
                return context.GetObject(perfectMatchedObjectName.Count > 0 ? perfectMatchedObjectName.Last() : objectNames[objectNames.Count -1]);
            }

            // if service type is IEnumerable<T>
            var elementType = serviceType.GetEnumerableElementType();
            if (elementType != null)
            {
                var enumerableServices = context.GetObjectsOfType(elementType);
                if (enumerableServices.Values.Count > 0 || context.ParentContext == null)
                {
                    return TypeConversionUtils.ConvertValueIfNecessary(serviceType, enumerableServices.Values, null);
                }
            }

            // if service type is generic type
            if (serviceType.IsGenericType && serviceType.IsConstructedGenericType)
            {
                var factory = ((IConfigurableApplicationContext)context).ObjectFactory;
                var openGenericType = serviceType.GetGenericTypeDefinition();
                var genericType = (Type)factory?.GetSingleton(SpringServiceProviderFactory.GenericTypePrefix + openGenericType.AssemblyQualifiedName);
                if (genericType != null)
                {
                    var closedServiceType = genericType.MakeGenericType(serviceType.GenericTypeArguments);
                    return _options.InstanceActivator(this, closedServiceType);
                }
            }

            return null;
        }

        private object LookupService(IApplicationContext context, Type serviceType)
        {
            var service = GetServiceIntetnal(context, serviceType);
            if (service == null && context.ParentContext != null)
            {
                return LookupService(context.ParentContext, serviceType);
            }
            return service;
        }
    }
}
