using HealthChecks.UI.Client;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWebApi.Application;
using MyWebApi.Application.Consumer;
using MyWebApi.Application.Saga;
using MyWebApi.Domain.Configurations;
using MyWebApi.Domain.Entities.Saga;
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
                .AddSqlServer(Configuration.GetConnectionString("MyWeb") ?? string.Empty, name: "MyWeb", tags: new[] { "sql" });

            // Logging
            services.AddLogging();

            // DI
            services.RegisterApplicationDependencies(Configuration);
            services.RegisterInfrastructureDependencies(Configuration);
            services.RegisterDatabase(Configuration);
            RegisterConsumer(services, Configuration);
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

        private void RegisterConsumer(IServiceCollection services, IConfiguration configuration)
        {
            //Add masstransit
            MessageBrokerOptions options = configuration.GetSection(MessageBrokerOptions.BindLocator).Get<MessageBrokerOptions>();
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.AddSagaStateMachine<MyStateMachine, Monitoring>(typeof(MyStateMachineDefinition))
                    .EntityFrameworkRepository(r =>
                    {
                        r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                        r.LockStatementProvider = new SqlServerLockStatementProvider();
                        r.ExistingDbContext<MonitoringDbContext>();
                    })
                    ;
                x.AddConsumer<OrderProcessConsumer>();
                x.AddConsumer<OrderSentMailConsumer>();

                x.UsingActiveMq((context, cfg) =>
                {
                    cfg.Host(options.Host, config =>
                    {
                        config.Username(options.User);
                        config.Password(options.Password);
                    });

                    cfg.ReceiveEndpoint($"order-process-{nameof(OrderProcessConsumer)}", e =>
                    {
                        e.ConfigureConsumer<OrderProcessConsumer>(context);
                        e.Bind<OrderProcessConsumer>();
                    });

                    cfg.ReceiveEndpoint($"order-send-mail-{nameof(OrderSentMailConsumer)}", e =>
                    {
                        e.ConfigureConsumer<OrderSentMailConsumer>(context);
                        e.Bind<OrderSentMailConsumer>();
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
