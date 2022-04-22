# Spring.Extensions.DependencyInjection

[![Build Status][ci-badge]][ci] [![Code Coverage][codecov-badge]][codecov]
[![NuGet version][nuget-badge]][nuget]

[ci]: https://github.com/akunzai/Spring.Extensions.DependencyInjection/actions?query=workflow%3ACI
[ci-badge]: https://github.com/akunzai/Spring.Extensions.DependencyInjection/workflows/CI/badge.svg
[codecov]: https://codecov.io/gh/akunzai/Spring.Extensions.DependencyInjection
[codecov-badge]: https://codecov.io/gh/akunzai/Spring.Extensions.DependencyInjection/branch/main/graph/badge.svg?token=KA1W0L496Y
[nuget]: https://www.nuget.org/packages/Spring.Extensions.DependencyInjection/
[nuget-badge]: https://img.shields.io/nuget/v/Spring.Extensions.DependencyInjection.svg?style=flat-square

Integrate [Spring.NET](https://github.com/spring-projects/spring-net) with [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection)

## Installation

```shell
dotnet add package Spring.Extensions.DependencyInjection
```

## Getting Started

integrate with Microsoft.Extensions.Hosting

```csharp
var host = Host.CreateDefaultBuilder()
.UseServiceProviderFactory(new SpringServiceProviderFactory())
.ConfigureServices((context, services) =>
{
    // ...
}).Build();
```

integrate with ASP.NET Core Minimal APIs

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new SpringServiceProviderFactory());
```

building ServiceProviderFactory from exists ApplicationContext

```csharp
using Microsoft.Extensions.DependencyInjection;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;
// ...
var factory = new SpringServiceProviderFactory(options =>
{
    var context = new CodeConfigApplicationContext();
    context.ScanAllAssemblies();
    context.Refresh();
    options.Parent = context;
});
// or
//var factory = new SpringServiceProviderFactory(ContextRegistry.GetContext());
// or
//var factory = new SpringServiceProviderFactory(new XmlApplicationContext("objects.xml"));
// ...
```

building ApplicationContext from ServiceCollection

```csharp
using Microsoft.Extensions.DependencyInjection;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;
// ...
var factory = new SpringServiceProviderFactory();
var services = new ServiceCollection();
ConfigureServices(services);
var context = factory.CreateBuilder(services);
// ...
```

building ServiceProvider from Spring.NET ApplicationContext

```csharp
using Microsoft.Extensions.DependencyInjection;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;
// ...
var factory = new SpringServiceProviderFactory();
var services = new ServiceCollection();
ConfigureServices(services);
var context = factory.CreateBuilder(services);
var provider = factory.CreateServiceProvider(context);
// or directly building serviceProvider from exists ApplicationContext without integrate ServiceCollection
//var provider = factory.CreateServiceProvider(ContextRegistry.GetContext());
// ...
```

## Known issues

- the same service type in CodeConfigApplicationContext or XmlApplicationContext was preferred for SpringServiceProvider in SpringServiceScope
