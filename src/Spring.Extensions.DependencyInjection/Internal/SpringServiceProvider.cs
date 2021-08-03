using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Spring.Context;
using Spring.Core.TypeConversion;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Support;

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

        private object GetServiceInternal(IApplicationContext context, Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider))
            {
                return this;
            }
            var objectNames = context.GetObjectNamesForType(serviceType);
            if (objectNames.Count > 0)
            {
                var perfectMatchedObjectNames = GetPerfectTypeMatchedObjectNames(context, serviceType, objectNames);
                var matchedObjectNames = perfectMatchedObjectNames.ToList();
                if (matchedObjectNames.Any())
                {
                    return context.GetObject(matchedObjectNames.Last());
                }
            }

            // if service type is IEnumerable<T>
            var elementType = serviceType.GetEnumerableElementType();
            if (elementType != null)
            {
                var perfectMatchedEnumerableObjectNames = GetPerfectTypeMatchedObjectNames(context, elementType);
                var matchedEnumerableObjectNames = perfectMatchedEnumerableObjectNames.ToList();
                if (matchedEnumerableObjectNames.Any() || context.ParentContext == null)
                {
                    var enumerableServices = new List<object>(matchedEnumerableObjectNames.Count);
                    foreach (var objectName in matchedEnumerableObjectNames)
                    {
                        enumerableServices.Add(context.GetObject(objectName));
                    }
                    return TypeConversionUtils.ConvertValueIfNecessary(serviceType, enumerableServices, null);
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
            var service = GetServiceInternal(context, serviceType);
            if (service == null && context.ParentContext != null)
            {
                return LookupService(context.ParentContext, serviceType);
            }
            return service;
        }

        private static IEnumerable<string> GetPerfectTypeMatchedObjectNames(IApplicationContext context, Type serviceType, IReadOnlyList<string> objectNames = null)
        {
            objectNames ??= context.GetObjectNamesForType(serviceType);
            var factory = ((IConfigurableApplicationContext)context).ObjectFactory;
            foreach (var objectName in objectNames)
            {
                var definition = factory.GetObjectDefinition(objectName);
                if (definition is AbstractObjectDefinition aod)
                {
                    if (aod.HasObjectType && aod.ObjectType == serviceType)
                    {
                        yield return objectName;
                    }
                    if (!aod.HasObjectType && !string.IsNullOrEmpty(aod.FactoryObjectName) && !string.IsNullOrEmpty(aod.FactoryMethodName))
                    {
                        var fod = factory.GetObjectDefinition(aod.FactoryObjectName);
                        if (fod != null)
                        {
                            var typeInfo = fod.ObjectType.GetTypeInfo();
                            var method = typeInfo.GetDeclaredMethod(aod.FactoryMethodName);
                            if (method != null && method.ReturnType == serviceType)
                            {
                                yield return objectName;
                            }
                        }
                    }
                }
                var singleton = factory.GetSingleton(objectName);
                if (singleton is IFactoryObject factoryObject && factoryObject.ObjectType == serviceType)
                {
                    yield return objectName;
                }
            }
        }
    }
}
