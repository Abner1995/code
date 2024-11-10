using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ContactSMS.WebAPI.HealthCheck;

public class RandomHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        int responseTimeInMs = Random.Shared.Next(300);

        if (responseTimeInMs < 100)
        {
            return Task.FromResult(HealthCheckResult.Healthy($"反应时间优等{responseTimeInMs}ms"));
        }
        else if (responseTimeInMs < 200)
        {
            return Task.FromResult(HealthCheckResult.Degraded($"反应时间良等{responseTimeInMs}ms"));
        }
        else
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"反应时间差等{responseTimeInMs}ms"));
        }
    }
}
