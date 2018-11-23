using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SampleShared;
using Spring.Context.Support;
using Spring.Extensions.DependencyInjection;

namespace SampleWebApp
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
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync($"Hello World from {provider.GetType().Name} !").ConfigureAwait(false);
                var command = provider.GetRequiredService<ICommand>();
                command.Execute();
            });
        }
    }
}
