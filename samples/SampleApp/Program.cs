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
                var command = provider.GetRequiredService<ICommand>();
                command.Execute();
            }

            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(logging => logging.AddConsole());
            services.AddTransient<IDateTimeProvider>(_ => new DummyDateTimeProvider
            {
                DateTime = DateTime.Now
            });
            services.AddTransient<ICommand, LoggerOutputDateTimeCommand>();
        }
    }
}
