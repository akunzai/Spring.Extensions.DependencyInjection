using System;
using Spring.Objects.Factory;

namespace Spring.Extensions.DependencyInjection.Internal;

internal class DelegateFactoryObject : IFactoryObject
{
    private readonly Func<object> _factory;

    public DelegateFactoryObject(Type objectType, Func<object> factory)
    {
        _factory = factory;
        ObjectType = objectType;
    }

    public Type ObjectType { get; }

    public bool IsSingleton { get; set; } = true;

    public object GetObject()
    {
        return _factory();
    }
}