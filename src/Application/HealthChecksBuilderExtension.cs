using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWebApi.Domain.Configurations;
using MyWebApi.Domain.HealthChecks;

namespace MyWebApi.Application
{
    public static class HealthCheckBuilderExtension
    {
        public static IHealthChecksBuilder AddApiDependencyHealthCheck(this IHealthChecksBuilder healthChecksBuilder, IConfiguration configuration)
        {
            HealthCheckTcpOptions tpcToCheck = configuration.GetSection(HealthCheckTcpOptions.ConfigurationBindName).Get<HealthCheckTcpOptions>();
            foreach (KeyValuePair<string, HealthCheckTcp> host in tpcToCheck)
            {
                healthChecksBuilder.AddTcpHealthCheck(setup => setup.AddHost(host.Value.Host, host.Value.Port)
                    , host.Key
                    , host.Value.FailureStatus
                );
            }

            Dictionary<string, HealthCheckUrls> urlsToCheck = configuration.GetSection(HealthCheckUrlsOptions.ConfigurationBindName).Get<Dictionary<string, HealthCheckUrls>>();
            foreach (KeyValuePair<string, HealthCheckUrls> url in urlsToCheck)
            {
                healthChecksBuilder.AddUrlGroup(url.Value.Url, url.Key, url.Value.FailureStatus);
            }

            return healthChecksBuilder;
        }
    }
}
