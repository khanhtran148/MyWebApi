using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyWebApi.Application.Abstractions;
using MyWebApi.Infrastructure.Persistence.Contexts;

namespace MyWebApi.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class InfrastructureDependencyInjection
    {
        public static void RegisterInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
        {
        }

        public static void RegisterDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            // use sqlServer
            services.AddDbContextPool<MyDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MyWeb"), op =>
                {
                    op.MigrationsAssembly(typeof(MyDbContext).Assembly.FullName);
                });
                options.UseLazyLoadingProxies();
            });

            services.AddDbContextPool<MonitoringDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MyWeb"), op =>
                {
                    op.MigrationsAssembly(typeof(MonitoringDbContext).Assembly.FullName);
                });
                options.UseLazyLoadingProxies();
            });

            // use postgres
            // services.AddDbContextPool<MyDbContext>(options =>
            // {
            //     options.UseNpgsql(configuration.GetConnectionString("MyWebPostgres"), op =>
            //     {
            //         op.MigrationsAssembly(typeof(MyDbContext).Assembly.FullName);
            //     });
            //     options.UseLazyLoadingProxies();
            // });

            services.TryAddScoped<IMyDbContextProcedures, MyDbContextProcedures>();
            services.TryAddScoped<IMyDbContext, MyDbContext>();
            services.TryAddScoped<IMonitoringDbContext, MonitoringDbContext>();
        }
    }
}
