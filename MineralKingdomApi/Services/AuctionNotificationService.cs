using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MineralKingdomApi.DTOs.ShoppingCartDTOs;
using MineralKingdomApi.Repositories;

namespace MineralKingdomApi.Services
{
    public class AuctionNotificationService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AuctionNotificationService> _logger;

        public AuctionNotificationService(IServiceProvider serviceProvider, ILogger<AuctionNotificationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Auction Notification Service starting.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(10)); // Adjust the interval as needed

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("Executing scheduled work in Auction Notification Service.");
            await NotifyWinnersOfFinishedAuctions();
            _logger.LogInformation("Scheduled work completed.");
        }

        private async Task NotifyWinnersOfFinishedAuctions()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var auctionRepository = scope.ServiceProvider.GetRequiredService<IAuctionRepository>();
                var auctionService = scope.ServiceProvider.GetRequiredService<IAuctionService>();
                var bidService = scope.ServiceProvider.GetRequiredService<IBidService>();
                var shoppingCartRepository = scope.ServiceProvider.GetRequiredService<IShoppingCartRepository>();
                var cartItemRepository = scope.ServiceProvider.GetRequiredService<ICartItemRepository>();

                try
                {
                    var finishedAuctions = await auctionRepository.GetFinishedAndUnnotifiedAuctionsAsync();
                    _logger.LogInformation($"Found {finishedAuctions.Count()} finished auctions to process.");

                    foreach (var auction in finishedAuctions)
                    {
                        var winningBidResult = await auctionRepository.GetWinningBidForCompletedAuction(auction.Id);
                        if (winningBidResult.IsSuccess && winningBidResult.WinningBid != null)
                        {
                            await auctionService.NotifyWinner(winningBidResult.WinningBid.UserId, auction);
                            auction.IsWinnerNotified = true;
                            await auctionRepository.UpdateAuctionAsync(auction);
                            _logger.LogInformation($"Notified winner for auction {auction.Id}.");

                            // Add item to the winner's cart
                            var cart = await shoppingCartRepository.GetCartWithItemsByUserIdAsync(winningBidResult.WinningBid.UserId);
                            var cartId = cart.Id;
                            var mineralId = (int)auction?.MineralId;
                            CartItemDTO cartItemDTO = new CartItemDTO
                            {
                                Id = cartId,
                                MineralId = mineralId,
                            };
                            await cartItemRepository.CreateCartItemAsync(winningBidResult.WinningBid.UserId, cartItemDTO, cartId);
                            _logger.LogInformation($"Added auction item to the winner's cart for auction {auction.Id}.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while notifying winners of finished auctions and adding items to their carts.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Auction Notification Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
