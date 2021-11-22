using NodaTime;
using NodaTime.Testing;
using Spring.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new SpringServiceProviderFactory());
builder.Services.AddSingleton<IClock, SystemClock>();
if (builder.Configuration.GetValue<bool>("Clock:Dummy"))
{
    builder.Services.AddSingleton<IClock>(_ => FakeClock.FromUtc(2021, 1, 1));
}

var app = builder.Build();

app.MapGet("/", async context =>
{
    var clock = context.RequestServices.GetRequiredService<IClock>();
    await context.Response.WriteAsync($"Current time is {clock.GetCurrentInstant()}");
});

app.Run();
