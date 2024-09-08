using System;
using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MyWebApi.Domain.HealthChecks
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class HealthCheckUrls
    {
        public Uri Url { get; set; }
        public HealthStatus FailureStatus { get; set; }

        public override string ToString() => $"[{FailureStatus}] {Url}";

        private string GetDebuggerDisplay()
        {
            return $"{Url}";
        }
    }
}
