using System;
using System.Net;
using System.Net.Mail;
using MineralKingdomApi.DTOs.AuctionDTOs;
using MineralKingdomApi.DTOs.AuctionDTOs.YourNamespace.Models;
using MineralKingdomApi.DTOs.ShoppingCartDTOs;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;

namespace MineralKingdomApi.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IMineralRepository _mineralRepository;
        private readonly IConfiguration _configuration;

        public AuctionService(IAuctionRepository auctionRepository, IUserRepository userRepository, ICartItemRepository cartItemRepository, IShoppingCartRepository shoppingCartRepository, IMineralRepository mineralRepository, IConfiguration configuration)
        {
            _auctionRepository = auctionRepository;
            _userRepository = userRepository;
            _cartItemRepository = cartItemRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _mineralRepository = mineralRepository;
            _configuration = configuration;
        }

        public async Task<IEnumerable<AuctionResponseDTO>> GetAllAuctionsAsync()
        {
            var auctions = await _auctionRepository.GetAllAuctionsAsync();
            return auctions.Select(a => MapToDTO(a));
        }

        public async Task<Auction?> GetAuctionByIdAsync(int id)
        {
            return await _auctionRepository.GetAuctionByIdAsync(id);
        }


        public async Task CreateAuctionAsync(Auction auction)
        {
            if (auction == null)
            {
                throw new ArgumentNullException(nameof(auction));
            }
            auction.StartTime = auction.StartTime.ToUniversalTime();
            auction.EndTime = auction.EndTime.ToUniversalTime();
            await _auctionRepository.CreateAuctionAsync(auction);
        }

        public async Task UpdateAuctionAsync(Auction auction)
        {
            if (auction == null)
            {
                throw new ArgumentNullException(nameof(auction));
            }

            await _auctionRepository.UpdateAuctionAsync(auction);
        }

        public async Task DeleteAuctionAsync(Auction auction)
        {
            if (auction == null)
            {
                throw new ArgumentNullException(nameof(auction));
            }
            await _auctionRepository.DeleteAuctionAsync(auction);
        }

        public async Task<IEnumerable<Auction>> GetAuctionsByStatusAsync(int auctionStatusId)
        {
            var auctions = await _auctionRepository.GetAuctionsByStatusAsync(auctionStatusId);
            return auctions ?? Enumerable.Empty<Auction>();
        }

        public async Task<IEnumerable<Auction>> GetActiveAuctionsAsync()
        {
            return await _auctionRepository.GetActiveAuctionsAsync();
        }

        public async Task<IEnumerable<Auction>> GetAuctionsWithBidsAsync()
        {
            return await _auctionRepository.GetAuctionsWithBidsAsync();
        }

        public async Task<IEnumerable<Bid>> GetBidsForAuctionAsync(int auctionId)
        {
            var bids = await _auctionRepository.GetBidsForAuctionAsync(auctionId);
    return bids ?? Enumerable.Empty<Bid>();
        }

        public async Task<IEnumerable<Auction>> GetAuctionsForMineralAsync(int mineralId)
        {
            return await _auctionRepository.GetAuctionsForMineralAsync(mineralId);
        }

        public async Task<BidResult> GetWinningBidForCompletedAuction(int auctionId)
        {
            var bidResult = await _auctionRepository.GetWinningBidForCompletedAuction(auctionId);
            bool bidResultIsSuccess = bidResult.IsSuccess;
            var auction = await _auctionRepository.GetAuctionByIdAsync(auctionId);
            bool auctionWinnerNotified = auction.IsWinnerNotified;

            if (bidResultIsSuccess && !auctionWinnerNotified)
            {
                // get user here by userID and so we can send that user to NotifyWinner
                decimal newPrice = bidResult.WinningBid.Amount;
                Mineral mineralToBeBought = await _mineralRepository.GetMineralByIdAsync((int)auction?.MineralId);
                mineralToBeBought.Price = newPrice;
                await _mineralRepository.UpdateMineralAsync(mineralToBeBought);
                auction.Mineral.Price = newPrice;
                var userId = bidResult.WinningBid.UserId;
                var user = await _userRepository.GetUserByIdAsync(userId);
                
                await NotifyWinner(userId, auction);
                auction.IsWinnerNotified = true;
                await _auctionRepository.UpdateAuctionAsync(auction);
                var cart = await _shoppingCartRepository.GetCartWithItemsByUserIdAsync(userId);
                var cartId = cart.Id;
                var mineralId = (int)auction?.MineralId;
                CartItemDTO cartItemDTO = new CartItemDTO
                {
                    Id = cartId,
                    MineralId = mineralId,
                };
                await _cartItemRepository.CreateCartItemAsync(userId, cartItemDTO, cartId);
            }
            bidResult.WinningBid.Auction = null;
            return bidResult;
        }

        public async Task NotifyWinner(int winnerUserId, Auction auction)
        {
            var user = await _userRepository.GetUserByIdAsync(winnerUserId);
            User newUser = new User
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
            var frontendUrl = _configuration.GetValue<string>("FRONTENDURL");
            if (string.IsNullOrEmpty(frontendUrl))
            {
                throw new InvalidOperationException("Frontend URL is not configured.");
            }

            string auctionLink = $"{frontendUrl}/home";

            var fromAddress = new MailAddress("noreply@yourwebsite.com", "Your Website Name");
            var toAddress = new MailAddress(newUser.Email, newUser.FirstName + " " + newUser.LastName);
            const string subject = "Congratulations! You've won an auction";
            string body = $"Dear {newUser.FirstName},\n\n" +
                          $"Congratulations! You have won the auction for '{auction.Title}'.\n" +
                          $"Please login to view the auction details and complete your purchase: {auctionLink}\n\n" +
                          $"Best regards,\n" +
                          $"Your Website Team";

            var mailtrapUsername = Environment.GetEnvironmentVariable("MAILTRAP_USERNAME");
            var mailtrapPassword = Environment.GetEnvironmentVariable("MAILTRAP_PASSWORD");

            var smtp = new SmtpClient
            {
                Host = "smtp.mailtrap.io", // SMTP Host from MailTrap
                Port = 587, // SMTP Port from MailTrap
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mailtrapUsername, mailtrapPassword)

            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                await smtp.SendMailAsync(message);
            }
        }


        public async Task<BidResult> GetCurrentWinningBidForAuction(int auctionId)
        {
            return await _auctionRepository.GetCurrentWinningBidForAuction(auctionId);
        }

        private AuctionResponseDTO MapToDTO(Auction auction)
        {
            return new AuctionResponseDTO
            {
                Id = auction.Id,
                Title = auction.Title,
                Description = auction.Description,
                StartingPrice = auction.StartingPrice,
                StartTime = auction.StartTime,
                EndTime = auction.EndTime,
                MineralName = auction.Mineral?.Name,  // Assuming Mineral has a Name property
                AuctionStatusName = auction.AuctionStatus?.Status,  // Assuming AuctionStatus has a Name property
                BidCount = auction.Bids?.Count ?? 0,  // Assuming Auction has a Bids collection
                MineralId = auction.MineralId
            };
        }

        //private Auction MapToModel(CreateAuctionDTO auctionDto)
        //{
        //    return new Auction
        //    {
        //        Title = auctionDto.Title,
        //        Description = auctionDto.Description,
        //        StartingPrice = auctionDto.StartingPrice,
        //        StartTime = auctionDto.StartTime,
        //        EndTime = auctionDto.EndTime,
        //        MineralId = auctionDto.MineralId,
        //        AuctionStatusId = auctionDto.AuctionStatusId
        //    };
        //}
    }
}

