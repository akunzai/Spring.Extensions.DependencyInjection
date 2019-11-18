using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleShared;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;

namespace SampleGenericHostApp
{
    public static class Program
    {
        public static void Main()
        {
            var hostBuilder = new HostBuilder()
            .UseServiceProviderFactory(new SpringServiceProviderFactory(options =>
            {
                var context = new CodeConfigApplicationContext();
                context.ScanAllAssemblies();
                context.Refresh();
                options.Parent = context;
            }))
            .ConfigureServices(services =>
            {
                services.AddLogging(logging => logging.AddConsole());
                var dummyClock = Environment.GetEnvironmentVariable("SYSTEM_CLOCK");
                if (!string.IsNullOrWhiteSpace(dummyClock) && DateTime.TryParse(dummyClock, out var dummyDateTime))
                {
                    services.AddSingleton<ISystemClock>(_ => new DummySystemClock(dummyDateTime));
                }
            });

            using (var host = hostBuilder.Build())
            {
                var clock = host.Services.GetRequiredService<ISystemClock>();
                Console.WriteLine($"Current DateTime is {clock.Now:O}");
            }

            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
        }
    }
}
