using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWebApi.Application;
using MyWebApi.Infrastructure;

namespace MyWebApi.WorkerService
{
    internal class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Options
            services.AddOptions();

            // HealthCheck
            services.AddHealthChecks()
                //.AddApiDependencyHealthCheck(Configuration)
                .AddProcessAllocatedMemoryHealthCheck(int.Parse(Configuration["HealthChecks:MaximumAllocatedMemory"] ?? string.Empty))
                .AddSqlServer(Configuration.GetConnectionString("SMARTAmaris") ?? string.Empty, name: "SMART_Amaris", tags: new[] { "sql" });

            // Logging
            services.AddLogging();

            // DI
            services.RegisterApplicationDependencies(Configuration);
            services.RegisterInfrastructureDependencies(Configuration);
            services.RegisterDatabase(Configuration);
            services.RegisterConsumer(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }
}
