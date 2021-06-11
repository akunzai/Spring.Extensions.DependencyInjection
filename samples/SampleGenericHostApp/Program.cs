using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleShared;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;

namespace SampleGenericHostApp
{
    public static class Program
    {
        public static void Main()
        {
            var host = Host.CreateDefaultBuilder()
                .UseServiceProviderFactory(new SpringServiceProviderFactory(options =>
                {
                    var context = new CodeConfigApplicationContext();
                    context.ScanWithTypeFilter(t => t.Name.EndsWith("SpringConfiguration"));
                    context.Refresh();
                    options.Parent = context;
                })).ConfigureServices((context, services) =>
                {
                    if (context.Configuration["SystemClock"]?.Equals("Dummy", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        services.AddSingleton<ISystemClock, DummySystemClock>();
                    }
                }).Build();

            var clock = host.Services.GetRequiredService<ISystemClock>();
            Console.WriteLine($"Current DateTime is {clock.Now:O}");
        }
    }
}