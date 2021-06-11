using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleShared;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;

namespace SampleApp
{
    public static class Program
    {
        public static void Main()
        {
            var factory = new SpringServiceProviderFactory(options =>
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    options.Parent = ContextRegistry.GetContext();
                }
                else
                {
                    // Mono doesn't read app.config
                    var context = new CodeConfigApplicationContext();
                    context.ScanWithTypeFilter(t => t.Name.EndsWith("SpringConfiguration"));
                    context.Refresh();
                    options.Parent = context;
                }
            });
            var services = new ServiceCollection();
            ConfigureServices(services);
            var resolver = factory.CreateServiceProvider(factory.CreateBuilder(services));
            var clock = resolver.GetRequiredService<ISystemClock>();
            Console.WriteLine($"Current DateTime is {clock.Now:O}");
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json",
                    optional: true)
                .Build();
            services.AddSingleton<IConfiguration>(configuration);
            if (configuration["SystemClock"]?.Equals("Dummy", StringComparison.OrdinalIgnoreCase) == true)
            {
                services.AddSingleton<ISystemClock, DummySystemClock>();
            }
        }
    }
}