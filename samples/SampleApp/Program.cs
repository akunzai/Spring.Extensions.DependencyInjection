using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleShared;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;

namespace SampleApp
{
    public static class Program
    {
        public static void Main()
        {
            var factory = new SpringServiceProviderFactory(ContextRegistry.GetContext());
            var services = new ServiceCollection();
            ConfigureServices(services);
            var context = factory.CreateBuilder(services);
            var provider = factory.CreateServiceProvider(context);

            using (provider as IDisposable)
            {
                var clock = provider.GetRequiredService<ISystemClock>();
                Console.WriteLine($"Current DateTime is {clock.Now:O}");
            }

            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(logging => logging.AddConsole());
            var dummyClock = Environment.GetEnvironmentVariable("SYSTEM_CLOCK");
            if (!string.IsNullOrWhiteSpace(dummyClock) && DateTime.TryParse(dummyClock, out var dummyDateTime))
            {
                services.AddSingleton<ISystemClock>(_ => new DummySystemClock(dummyDateTime));
            }
        }
    }
}
