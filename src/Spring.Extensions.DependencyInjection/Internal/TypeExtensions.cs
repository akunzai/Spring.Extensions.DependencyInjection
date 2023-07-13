using System;
using System.Collections.Generic;
using System.Linq;

namespace Spring.Extensions.DependencyInjection.Internal;

internal static class TypeExtensions
{
    public  static Type GetEnumerableElementType(this Type type)
    {
        // Type is Array
        // short-circuit if you expect lots of arrays 
        if (type.IsArray)
            return type.GetElementType();

        // type is IEnumerable<T>;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return type.GetGenericArguments()[0];

        // type implements/extends IEnumerable<T>;
        return type.GetInterfaces()
            .Where(t => t.IsGenericType
                        && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(t => t.GenericTypeArguments[0]).FirstOrDefault();
    }
}