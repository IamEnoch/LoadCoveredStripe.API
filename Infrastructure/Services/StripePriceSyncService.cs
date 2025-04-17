using LoadCoveredStripe.API.Infrastructure.Interfaces;

namespace LoadCoveredStripe.API.Infrastructure.Services;

public class StripePriceSyncService(
    IServiceProvider serviceProvider,
    ILogger<StripePriceSyncService> logger)
    : BackgroundService
{
    private readonly TimeSpan _syncInterval = TimeSpan.FromHours(24); // Sync once per day

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Stripe Price Sync Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncPricesAsync();
                
                // Wait for the next sync interval
                await Task.Delay(_syncInterval, stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "Error occurred while syncing Stripe prices.");
                
                // Wait a shorter time before retrying after an error
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }   
    
    private async Task SyncPricesAsync()
    {
        logger.LogInformation("Syncing Stripe prices...");

        using var scope = serviceProvider.CreateScope();
        var stripeService = scope.ServiceProvider.GetRequiredService<IStripeService>();

        var result = await stripeService.SyncPricesAsync();

        if (result.IsSuccess)
        {
            logger.LogInformation("Successfully synced prices from Stripe.");
        }
        else
        {
            logger.LogError("Failed to sync prices from Stripe: {Message}", result.Message);
        }
    }
}
