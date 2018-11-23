using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Spring.Context;

namespace Spring.Extensions.DependencyInjection.Internal
{
    internal static class ActivatorHelper
    {
        /// <summary>
        /// Try Create Instance with the constructor that most parameters matched
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="instanceType"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static bool TryCreateInstance(IServiceProvider provider, Type instanceType, out object instance)
        {
            var constructors = instanceType
                .GetTypeInfo()
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length > 1)
            {
                var context = provider.GetService<IApplicationContext>();
                var factory = ((IConfigurableApplicationContext)context).ObjectFactory;
                // find the constructor with the most parameters first
                var bestMatchedLength = -1;
                ConstructorInfo bestMatchedCtor = null;
                foreach (var ctorCandidate in constructors.OrderByDescending(c => c.GetParameters().Length))
                {
                    var paramTypes = ctorCandidate.GetParameters().Select(p => p.ParameterType).ToArray();
                    if (paramTypes.Length == 0)
                    {
                        // parameter-less constructor
                        continue;
                    }
                    var paramMatches = new bool[paramTypes.Length];
                    for (var i = 0; i < paramTypes.Length; i++)
                    {
                        var paramType = paramTypes[i];
                        // if service type is IEnumerable<T>
                        var elementType = paramType.GetEnumerableElementType();
                        if (elementType != null && factory.GetObjectNamesForType(elementType).Count > 0)
                        {
                            paramMatches[i] = true;
                        }
                        // if service type is generic type
                        else if (paramType.IsGenericType && paramType.IsConstructedGenericType && factory.GetSingleton(SpringServiceProviderFactory.GenericTypePrefix + paramType.GetGenericTypeDefinition().AssemblyQualifiedName) != null)
                        {
                            paramMatches[i] = true;
                        }
                        else if (factory.GetObjectNamesForType(paramType).Count > 0)
                        {
                            paramMatches[i] = true;
                        }
                        else
                        {
                            continue;
                        }

                        if (i == paramTypes.Length - 1)
                        {
                            var matchedLength = paramMatches.Count(x => x);
                            if (bestMatchedLength < matchedLength)
                            {
                                bestMatchedLength = matchedLength;
                                bestMatchedCtor = ctorCandidate;
                            }
                        }
                    }
                }
                if (bestMatchedCtor != null)
                {
                    var parameters = bestMatchedCtor.GetParameters().Select(p => provider.GetService(p.ParameterType)).ToArray();
                    instance = ActivatorUtilities.CreateInstance(provider, bestMatchedCtor.DeclaringType, parameters);
                    return true;
                }
            }
            instance = null;
            return false;
        }
    }
}
