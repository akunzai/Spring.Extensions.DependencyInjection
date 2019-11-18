using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SampleShared;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;

namespace SampleWebHostApp
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var factory = new SpringServiceProviderFactory(options =>
            {
                var context = new CodeConfigApplicationContext();
                context.ScanAllAssemblies();
                context.Refresh();
                options.Parent = context;
            });
            var dummyClock = Environment.GetEnvironmentVariable("SYSTEM_CLOCK");
            if (!string.IsNullOrWhiteSpace(dummyClock) && DateTime.TryParse(dummyClock, out var dummyDateTime))
            {
                services.AddSingleton<ISystemClock>(_ => new DummySystemClock(dummyDateTime));
            }
            var containerBuilder = factory.CreateBuilder(services);
            return factory.CreateServiceProvider(containerBuilder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.Run(async context =>
            {
                var clock = provider.GetRequiredService<ISystemClock>();
                await context.Response.WriteAsync($"Current DateTime is {clock.Now:O}").ConfigureAwait(false);
            });
        }
    }
}
