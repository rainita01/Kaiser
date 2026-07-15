using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Data_Layer.Context;
using Microsoft.EntityFrameworkCore;

namespace Core_Layer.Services.BackGroundServices;

public class LogCleanupService(IServiceProvider serviceProvider, ILogger<LogCleanupService> logger)
    : BackgroundService
{
    private readonly TimeSpan interval = TimeSpan.FromDays(1); // check once a day

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<Context>();

                var cutoff = DateTime.UtcNow.AddDays(-10);

                var deletedCount = await context.Database.ExecuteSqlInterpolatedAsync(
                    $"DELETE FROM Logs WHERE TimeStamp < {cutoff}", stoppingToken);

                logger.LogInformation("Log cleanup ran: removed {Count} rows older than 10 days", deletedCount);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Log cleanup job failed");
            }

            await Task.Delay(interval, stoppingToken); // wait 24h before next run
        }
    }
}