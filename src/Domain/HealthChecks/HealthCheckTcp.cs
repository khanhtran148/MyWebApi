using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyWebApi.Domain.HealthChecks
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class HealthCheckTcp
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public HealthStatus FailureStatus { get; set; }

        public override string ToString() => $"[{FailureStatus}] {Host}:{Port}";

        private string GetDebuggerDisplay()
        {
            return $"{Host}:{Port}";
        }
    }
}
