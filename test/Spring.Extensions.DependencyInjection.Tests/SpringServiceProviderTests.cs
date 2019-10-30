using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Spring.Extensions.DependencyInjection.Tests.Fakes;
using Xunit;

namespace Spring.Extensions.DependencyInjection.Tests
{
    public class SpringServiceProviderTests
    {
        [Fact]
        public void ServicesRegisteredWithImplementationTypeCanBeResolved()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddTransient<IFakeService, FakeService>();
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var service = provider.GetService<IFakeService>();

            // Assert
            Assert.NotNull(service);
            Assert.IsType<FakeService>(service);
        }

        [Fact]
        public void ServicesRegisteredWithImplementationType_ReturnDifferentInstancesPerResolution_ForTransientServices()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddTransient(typeof(IFakeService), typeof(FakeService));
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var service1 = provider.GetService<IFakeService>();
            var service2 = provider.GetService<IFakeService>();

            // Assert
            Assert.IsType<FakeService>(service1);
            Assert.IsType<FakeService>(service2);
            Assert.NotSame(service1, service2);
        }

        [Fact]
        public void ServicesRegisteredWithImplementationType_ReturnSameInstancesPerResolution_ForSingletons()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddSingleton(typeof(IFakeService), typeof(FakeService));
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var service1 = provider.GetService<IFakeService>();
            var service2 = provider.GetService<IFakeService>();

            // Assert
            Assert.IsType<FakeService>(service1);
            Assert.IsType<FakeService>(service2);
            Assert.Same(service1, service2);
        }

        [Fact]
        public void ServiceInstanceCanBeResolved()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            var instance = new FakeService();
            collection.AddSingleton(typeof(IFakeServiceInstance), instance);
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var service = provider.GetService<IFakeServiceInstance>();

            // Assert
            Assert.Same(instance, service);
        }

        [Fact]
        public void TransientServiceCanBeResolvedFromProvider()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddTransient(typeof(IFakeService), typeof(FakeService));
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var service1 = provider.GetService<IFakeService>();
            var service2 = provider.GetService<IFakeService>();

            // Assert
            Assert.NotNull(service1);
            Assert.NotSame(service1, service2);
        }

        [Fact]
        public void TransientServiceCanBeResolvedFromScope()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddTransient(typeof(IFakeService), typeof(FakeService));
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var service1 = provider.GetService<IFakeService>();

            using var scope = provider.CreateScope();
            var scopedService1 = scope.ServiceProvider.GetService<IFakeService>();
            var scopedService2 = scope.ServiceProvider.GetService<IFakeService>();

            // Assert
            Assert.NotSame(service1, scopedService1);
            Assert.NotSame(service1, scopedService2);
            Assert.NotSame(scopedService1, scopedService2);
        }

        [Fact]
        public void SingleServiceCanBeIEnumerableResolved()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddTransient<IFakeService, FakeService>();
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var services = provider.GetService<IEnumerable<IFakeService>>();

            // Assert
            Assert.NotNull(services);
            var service = Assert.Single(services);
            Assert.IsType<FakeService>(service);
        }

        [Fact]
        public void MultipleServiceCanBeIEnumerableResolved()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddTransient(typeof(IFakeMultipleService), typeof(FakeOneMultipleService));
            collection.AddTransient(typeof(IFakeMultipleService), typeof(FakeTwoMultipleService));
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var services = provider.GetService<IEnumerable<IFakeMultipleService>>();

            // Assert
            Assert.Collection(services.OrderBy(s => s.GetType().FullName),
                service => Assert.IsType<FakeOneMultipleService>(service),
                service => Assert.IsType<FakeTwoMultipleService>(service));
        }

        [Fact]
        public void OuterServiceCanHaveOtherServicesInjected()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            var fakeService = new FakeService();
            collection.AddSingleton<IFakeService>(fakeService);
            collection.AddTransient<IFakeOuterService, FakeOuterService>();
            collection.AddTransient<IFakeMultipleService, FakeOneMultipleService>();
            collection.AddTransient<IFakeMultipleService, FakeTwoMultipleService>();
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var outerservice = provider.GetService<IFakeOuterService>();

            // Assert
            Assert.Same(fakeService, outerservice.SingleService);
            Assert.Collection(outerservice.MultipleServices.OrderBy(s => s.GetType().FullName),
                service => Assert.IsType<FakeOneMultipleService>(service),
                service => Assert.IsType<FakeTwoMultipleService>(service));
        }

        [Fact]
        public void ServiceWithOptionalDependencyCanBeResolved()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            var fakeService = new FakeService();
            collection.AddSingleton<IFakeService>(fakeService);
            collection.AddTransient<IFakeOuterService, FakeOuterServiceWithAmbiguousCtors>();
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var outerservice = provider.GetService<IFakeOuterService>();

            // Assert
            Assert.NotNull(outerservice);
            Assert.Same(fakeService, outerservice.SingleService);
        }

        [Fact]
        public void MostParameterCtorPreferredOverParameterLessCtor()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            var fakeService = new FakeService();
            collection.AddSingleton<IFakeService>(fakeService);
            collection.AddTransient<IFakeOuterService, FakeOuterServiceWithAmbiguousCtors>();
            collection.AddTransient<IFakeMultipleService, FakeOneMultipleService>();
            collection.AddTransient<IFakeMultipleService, FakeTwoMultipleService>();
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var outerservice = provider.GetService<IFakeOuterService>();

            // Assert
            Assert.NotNull(outerservice);
            Assert.Same(fakeService, outerservice.SingleService);
            Assert.Collection(outerservice.MultipleServices.OrderBy(s => s.GetType().FullName),
                service => Assert.IsType<FakeOneMultipleService>(service),
                service => Assert.IsType<FakeTwoMultipleService>(service));
        }

        [Fact]
        public void FactoryServicesCanBeCreatedByGetService()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddTransient<IFakeService, FakeService>();
            collection.AddTransient<IFactoryService>(p =>
            {
                var fakeService = p.GetRequiredService<IFakeService>();
                return new TransientFactoryService
                {
                    FakeService = fakeService,
                    Value = 42
                };
            });
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var service = provider.GetService<IFactoryService>();

            // Assert
            Assert.NotNull(service);
            Assert.Equal(42, service.Value);
            Assert.NotNull(service.FakeService);
            Assert.IsType<FakeService>(service.FakeService);
        }

        [Fact]
        public void FactoryServicesAreCreatedAsPartOfCreatingObjectGraph()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddTransient<IFakeService, FakeService>();
            collection.AddTransient<IFactoryService>(p =>
            {
                var fakeService = p.GetService<IFakeService>();
                return new TransientFactoryService
                {
                    FakeService = fakeService,
                    Value = 42
                };
            });
            collection.AddScoped(p =>
            {
                var fakeService = p.GetService<IFakeService>();
                return new ScopedFactoryService
                {
                    FakeService = fakeService,
                };
            });
            collection.AddTransient<ServiceAcceptingFactoryService>();
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var service1 = provider.GetService<ServiceAcceptingFactoryService>();
            var service2 = provider.GetService<ServiceAcceptingFactoryService>();

            // Assert
            Assert.Equal(42, service1.TransientService.Value);
            Assert.NotNull(service1.TransientService.FakeService);

            Assert.Equal(42, service2.TransientService.Value);
            Assert.NotNull(service2.TransientService.FakeService);

            Assert.NotNull(service1.ScopedService.FakeService);

            // Verify scoping works
            Assert.NotSame(service1.TransientService, service2.TransientService);
            Assert.Same(service1.ScopedService, service2.ScopedService);
        }

        [Fact]
        public void LastServiceReplacesPreviousServices()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddTransient<IFakeMultipleService, FakeOneMultipleService>();
            collection.AddTransient<IFakeMultipleService, FakeTwoMultipleService>();
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var service = provider.GetService<IFakeMultipleService>();

            // Assert
            Assert.IsType<FakeTwoMultipleService>(service);
        }

        [Fact]
        public void OpenGenericServicesCanBeResolved()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddTransient(typeof(IFakeOpenGenericService<>), typeof(FakeOpenGenericService<>));
            collection.AddSingleton<IFakeSingletonService, FakeService>();
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var genericService = provider.GetService<IFakeOpenGenericService<IFakeSingletonService>>();
            var singletonService = provider.GetService<IFakeSingletonService>();

            // Assert
            Assert.NotNull(genericService);
            Assert.Same(singletonService, genericService.Value);
        }

        [Fact]
        public void ClosedServicesPreferredOverOpenGenericServices()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddTransient(typeof(IFakeOpenGenericService<PocoClass>), typeof(FakeService));
            collection.AddTransient(typeof(IFakeOpenGenericService<>), typeof(FakeOpenGenericService<>));
            collection.AddSingleton<PocoClass>();
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var service = provider.GetService<IFakeOpenGenericService<PocoClass>>();

            // Assert
            Assert.IsType<FakeService>(service);
        }

        [Fact]
        public void AttemptingToResolveNonexistentServiceReturnsNull()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var service = provider.GetService<INonexistentService>();

            // Assert
            Assert.Null(service);
        }

        [Fact]
        public void NonexistentServiceCanBeIEnumerableResolved()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var services = provider.GetService<IEnumerable<INonexistentService>>();

            // Assert
            Assert.Empty(services);
        }

        [Fact]
        public void OnlyPerfectTypeMatchedServiceCanBeResolved()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddTransient<IFakeService, FakeService>();
            collection.AddTransient<FakeService, FakeService>();
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var service1 = provider.GetService<IFakeService>();
            var service2 = provider.GetService<IFakeEveryService>();

            // Assert
            Assert.NotNull(service1);
            Assert.Null(service2);
        }

        [Fact]
        public void OnlyPerfectTypeMatchedServiceCanBeIEnumerableResolved()
        {
            // Arrange
            var factory = new SpringServiceProviderFactory();
            var collection = new TestServiceCollection();
            collection.AddTransient<IFakeService, FakeService>();
            collection.AddTransient<FakeService, FakeService>();
            var provider = factory.CreateServiceProvider(factory.CreateBuilder(collection));

            // Act
            var services = provider.GetService<IEnumerable<IFakeService>>();

            // Assert
            Assert.Single(services);
        }
    }
}
