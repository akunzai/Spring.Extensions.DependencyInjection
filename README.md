# Spring.Extensions.DependencyInjection

integrate [Spring.NET](https://github.com/spring-projects/spring-net) with [Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/DependencyInjection)

[![NuGet version](https://img.shields.io/nuget/v/Spring.Extensions.DependencyInjection.svg?style=flat-square)](https://www.nuget.org/packages/Spring.Extensions.DependencyInjection/)
[![Build status](https://ci.appveyor.com/api/projects/status/cne5xd6c9dbmbxtr?svg=true)](https://ci.appveyor.com/project/akunzai/spring-extensions-dependencyinjection)

## Installation

```shell
# Package Manager
Install-Package Spring.Extensions.DependencyInjection

# .NET CLI
dotnet add package Spring.Extensions.DependencyInjection
```

## Known issues

- currently, Spring.NET 2.0 still required full .NET Framework, but [Spring.NET 3.0 will support .NET Core 2.0](https://github.com/spring-projects/spring-net/issues/133)
- the same service type in CodeConfigApplicationContext or XmlApplicationContext was preferred for SpringServiceProvider in SpringServiceScope

## Getting Started

building ServiceProviderFactory from exists ApplicationContext

```csharp
using Microsoft.Extensions.DependencyInjection;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;
...
    public static void Main()
    {
        var factory = new SpringServiceProviderFactory(options =>
        {
            var parent = new CodeConfigApplicationContext();
            parent.ScanAllAssemblies();
            parent.Refresh();
        });
        // or
        //var factory = new SpringServiceProviderFactory(ContextRegistry.GetContext());
        // or
        //var factory = new SpringServiceProviderFactory(new XmlApplicationContext("objects.xml"));
    }
...
```

building ApplicationContext from ServiceCollection

```csharp
using Microsoft.Extensions.DependencyInjection;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;
...
    public static void Main()
    {
        var factory = new SpringServiceProviderFactory();
        var services = new ServiceCollection();
        ConfigureServices(services);
        var context = factory.CreateBuilder(services);
    }
    private static void ConfigureServices(ServiceCollection services)
    {
        ...
    }
...
```

building ServiceProvider from Spring.NET ApplicationContext

```csharp
using Microsoft.Extensions.DependencyInjection;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;
...
    public static void Main()
    {
        var factory = new SpringServiceProviderFactory();
        var services = new ServiceCollection();
        ConfigureServices(services);
        var context = factory.CreateBuilder(services);
        var provider = factory.CreateServiceProvider(context);
    	// or directly building serviceProvider from exists ApplicationContext without integrae ServiceCollection
        //var provider = factory.CreateServiceProvider(ContextRegistry.GetContext());
    }
...
```

finally, you can integrate `Spring.NET` application with `Microsoft.Extensions.DependencyInjection`.