using System;
using Microsoft.Extensions.DependencyInjection;
using Spring.Context;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection.Internal;
using Xunit;

namespace Spring.Extensions.DependencyInjection.Tests;

public class SpringServiceProviderFactoryTests
{
    [Fact]
    public void CreateBuilderReturnsNewInstance()
    {
        // Arrange
        var factory = new SpringServiceProviderFactory();

        // Act
        var builder = factory.CreateBuilder(new TestServiceCollection());

        // Assert
        Assert.NotNull(builder);
        Assert.IsAssignableFrom<IApplicationContext>(builder);
    }

    [Fact]
    public void CreateBuilderWithParentContextWhenProvided()
    {
        // Arrange
        var parent = new CodeConfigApplicationContext();
        parent.ScanAllAssemblies();
        parent.Refresh();
        var factory = new SpringServiceProviderFactory(parent);

        // Act
        var builder = factory.CreateBuilder(new TestServiceCollection());

        // Assert
        Assert.Same(parent, builder.ParentContext);
    }

    [Fact]
    public void CreateServiceProviderReturnsNewInstance()
    {
        // Arrange
        var factory = new SpringServiceProviderFactory();
        var builder = factory.CreateBuilder(new TestServiceCollection());

        // Act
        var provider = factory.CreateServiceProvider(builder);

        // Assert
        Assert.NotNull(provider);
        Assert.IsAssignableFrom<IServiceProvider>(provider);
    }

    [Fact]
    public void CreateServiceProviderContainsApplicationContext()
    {
        // Arrange
        var factory = new SpringServiceProviderFactory();
        var builder = factory.CreateBuilder(new TestServiceCollection());
        var provider = factory.CreateServiceProvider(builder);

        // Act
        var applicationContext = provider.GetService<IApplicationContext>();

        // Assert
        Assert.NotNull(applicationContext);
    }

    [Fact]
    public void CreateServiceProviderContainsServiceProviderFactory()
    {
        // Arrange
        var factory = new SpringServiceProviderFactory();
        var builder = factory.CreateBuilder(new TestServiceCollection());
        var provider = factory.CreateServiceProvider(builder);

        // Act
        var serviceProviderFactory = provider.GetService<IServiceProviderFactory<IApplicationContext>>();

        // Assert
        Assert.NotNull(serviceProviderFactory);
        Assert.Same(factory, serviceProviderFactory);
    }

    [Fact]
    public void CreateServiceProviderContainsSelf()
    {
        // Arrange
        var factory = new SpringServiceProviderFactory();
        var builder = factory.CreateBuilder(new TestServiceCollection());
        var provider = factory.CreateServiceProvider(builder);

        // Act
        var serviceProvider = provider.GetService<IServiceProvider>();

        // Assert
        Assert.NotNull(serviceProvider);
        Assert.Same(provider, serviceProvider);
    }

    [Fact]
    public void CreateServiceProviderWithContextContainsServiceScopeFactory()
    {
        // Arrange
        var factory = new SpringServiceProviderFactory();
        var provider = factory.CreateServiceProvider(new GenericApplicationContext());

        // Act
        var serviceScopeFactory = provider.GetService<IServiceScopeFactory>();

        // Assert
        Assert.NotNull(serviceScopeFactory);
        Assert.IsAssignableFrom<IServiceScopeFactory>(serviceScopeFactory);
    }
}