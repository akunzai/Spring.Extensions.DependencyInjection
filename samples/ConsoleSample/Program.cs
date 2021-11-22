using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Testing;
using Spring.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder()
.UseServiceProviderFactory(new SpringServiceProviderFactory())
.ConfigureServices((context, services) =>
{
    services.AddSingleton<IClock, SystemClock>();
    if (context.Configuration.GetValue<bool>("Clock:Dummy"))
    {
        services.AddSingleton<IClock>(_ => FakeClock.FromUtc(2021, 1, 1));
    }
}).Build();

var clock = host.Services.GetRequiredService<IClock>();
Console.WriteLine($"Current time is {clock.GetCurrentInstant()}");
await Task.Delay(TimeSpan.FromSeconds(5));
Console.WriteLine($"Current time is {clock.GetCurrentInstant()}");
