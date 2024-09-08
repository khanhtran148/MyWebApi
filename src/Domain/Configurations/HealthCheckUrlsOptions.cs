using System.Collections.Generic;
using MyWebApi.Domain.HealthChecks;

namespace MyWebApi.Domain.Configurations
{
    public sealed class HealthCheckUrlsOptions
        : Dictionary<string, HealthCheckUrls>
    {
        public const string ConfigurationBindName = "HealthChecks:HealthChecksUrls";
    }
}
