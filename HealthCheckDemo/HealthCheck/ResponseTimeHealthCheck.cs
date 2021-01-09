using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using YamlDotNet.Serialization.TypeInspectors;

namespace HealthCheckDemo.HealthCheck
{
    public class ResponseTimeHealthCheck : IHealthCheck
    {
        private Random rnd = new Random();
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            int ResponseTimeInMS = rnd.Next(1, 300);

            if (ResponseTimeInMS < 100)
            {
                return
                    Task.FromResult(HealthCheckResult.Healthy($"The response time looks good ({ResponseTimeInMS}ms)."));
            }
            else if (ResponseTimeInMS < 200)
            {
                return
                    Task.FromResult(HealthCheckResult.Degraded($"The response time is a bit slow ({ResponseTimeInMS}ms)."));
            }
            else
            {
                return 
                Task.FromResult(HealthCheckResult.Unhealthy($"The response time is  unacceptable({ResponseTimeInMS}ms)"));
            }
        }
    }

}
