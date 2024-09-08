using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWebApi.Application.Common;
using MyWebApi.Domain.Configurations;

namespace MyWebApi.Application
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationDependencyInjection
    {
        public static void RegisterApplicationDependencies(this IServiceCollection services, IConfiguration configuration, bool isWorkerService = false)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(ApplicationDependencyInjection).GetTypeInfo().Assembly);
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssembly(typeof(ApplicationDependencyInjection).GetTypeInfo().Assembly);

            // Add mapster
            TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
            if (isWorkerService)
            {
                config.Scan(Assembly.Load("MyWebApi.Api.Server"),
                    Assembly.Load("MyWebApi.Application"),
                    Assembly.Load("MyWebApi.Infrastructure"));
            }
            else
            {
                config.Scan(
                    Assembly.Load("MyWebApi.Application"),
                    Assembly.Load("MyWebApi.Infrastructure"));
            }
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();
        }

        public static void RegisterActiveMqPublisher(this IServiceCollection services, IConfiguration configuration)
        {
            MessageBrokerOptions options = configuration.GetSection(MessageBrokerOptions.BindLocator).Get<MessageBrokerOptions>();

            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                // use activeMQ
                x.UsingActiveMq((context, cfg) =>
                {
                    cfg.Host(options.Host, c =>
                    {
                        c.Username(options.User);
                        c.Password(options.Password);
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });
        }

        public static void RegisterRabbitMqPublisher(this IServiceCollection services, IConfiguration configuration)
        {
            MessageBrokerOptions options = configuration.GetSection(MessageBrokerOptions.BindLocator).Get<MessageBrokerOptions>();
            services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                // use rabbitMQ
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(options.Host, options.VirtualHost, c =>
                    {
                        c.Username(options.User);
                        c.Password(options.Password);
                    });
                });

            });
        }
    }
}
