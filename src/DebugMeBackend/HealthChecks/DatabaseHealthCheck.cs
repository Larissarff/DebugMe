using DebugMeBackend.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DebugMeBackend.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _context;

    public DatabaseHealthCheck(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            bool canConnect = await _context.Database.CanConnectAsync(cancellationToken);

            if (!canConnect)
            {
                return HealthCheckResult.Unhealthy(
                    description: "Database is not accessible");
            }

            return HealthCheckResult.Healthy(
                description: "Database is connected and responding");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                description: "Database check failed",
                exception: ex);
        }
    }
}
