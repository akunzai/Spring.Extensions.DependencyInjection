using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleShared;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;

namespace SampleSpringApp
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
                services.AddTransient<IDateTimeProvider>(_ => new DummyDateTimeProvider
                {
                    DateTime = DateTime.Now
                });
                services.AddTransient<ICommand, LoggerOutputDateTimeCommand>();
            });

            using (var host = hostBuilder.Build())
            {
                var command = host.Services.GetRequiredService<ICommand>();
                command.Execute();
            }

            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
        }
    }
}
