using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Spring.Extensions.DependencyInjection.Tests
{
    internal class TestServiceCollection : List<ServiceDescriptor>, IServiceCollection
    {
    }
}