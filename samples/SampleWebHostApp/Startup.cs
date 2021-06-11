using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleShared;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;

namespace SampleWebHostApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var factory = new SpringServiceProviderFactory(options =>
            {
                var context = new CodeConfigApplicationContext();
                context.ScanWithTypeFilter(t => t.Name.EndsWith("SpringConfiguration"));
                context.Refresh();
                options.Parent = context;
            });
            if (Configuration["SystemClock"]?.Equals("Dummy", StringComparison.OrdinalIgnoreCase) == true)
            {
                services.AddSingleton<ISystemClock, DummySystemClock>();
            }
            return factory.CreateServiceProvider(factory.CreateBuilder(services));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.Run(async context =>
            {
                var clock = services.GetRequiredService<ISystemClock>();
                await context.Response.WriteAsync($"Current DateTime is {clock.Now:O}").ConfigureAwait(false);
            });
        }
    }
}
