using System;
using Microsoft.Extensions.DependencyInjection;
using Spring.Context;
using Spring.Context.Support;

namespace Spring.Extensions.DependencyInjection.Internal;

internal class SpringServiceScopeFactory : IServiceScopeFactory
{
    private readonly IApplicationContext _parent;
    private readonly SpringServiceProviderOptions _options;

    public SpringServiceScopeFactory(IApplicationContext parent, SpringServiceProviderOptions options)
    {
        _parent = parent;
        _options = options;
    }

    public IServiceScope CreateScope()
    {
        var context = new GenericApplicationContext($"{SpringServiceProviderFactory.ApplicationContextName}#{Guid.NewGuid().ToString()}", true, _parent);
        return new SpringServiceScope(context, _options);
    }
}