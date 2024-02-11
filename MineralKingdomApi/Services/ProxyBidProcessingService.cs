using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MineralKingdomApi.Services
{
    public class ProxyBidProcessingService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ProxyBidProcessingService> _logger;

        public ProxyBidProcessingService(IServiceProvider serviceProvider, ILogger<ProxyBidProcessingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Proxy Bid Processing Service starting.");
            _timer = new Timer(ProcessProxyBids, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(45)); // Adjust the interval as needed

            return Task.CompletedTask;
        }

        private async void ProcessProxyBids(object state)
        {
            _logger.LogInformation("Executing scheduled work in Proxy Bid Processing Service.");
            await HandleProxyBids();
            _logger.LogInformation("Scheduled work completed.");
        }

        private async Task HandleProxyBids()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var bidService = scope.ServiceProvider.GetRequiredService<IBidService>();

                try
                {
                    // Logic to fetch and process proxy bids
                    // This might involve calling a method similar to your existing HandleProxyBidAsync
                    await bidService.ProcessProxyBids();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing proxy bids.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Proxy Bid Processing Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
