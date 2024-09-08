using System.Collections.Generic;
using MyWebApi.Domain.HealthChecks;

namespace MyWebApi.Domain.Configurations
{
    public sealed class HealthCheckTcpOptions
        : Dictionary<string, HealthCheckTcp>
    {
        public const string ConfigurationBindName = "HealthChecks:TcpHealthChecks";
    }
}
