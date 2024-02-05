using System.Collections.Generic;
using System.Threading.Tasks;
using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.DTOs.BidDTOs;
using MineralKingdomApi.Exceptions;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;
using MineralKingdomApi.Services;
using Newtonsoft.Json;

public class BidService : IBidService
{
    private readonly IBidRepository _bidRepository;
    private readonly IAuctionRepository _auctionRepository;
    private readonly MineralKingdomContext _context;
    private readonly AppWebSocketsManager _appWebSocketManager;
    private readonly ILogger<BidService> _logger;

    public BidService(IBidRepository bidRepository, IAuctionRepository auctionRepository, MineralKingdomContext context, AppWebSocketsManager appWebSocketsManager, ILogger<BidService> logger)
    {
        _bidRepository = bidRepository;
        _auctionRepository = auctionRepository;
        _context = context;
        _appWebSocketManager = appWebSocketsManager;
        _logger = logger;
    }

    public async Task<IEnumerable<BidDTO>> GetAllBidsAsync()
    {
        var bids = await _bidRepository.GetAllBidsAsync();
        // Assuming a mapping from Bid to BidDTO, for simplicity using direct mapping here
        return bids.Select(b => new BidDTO
        {
            Id = b.Id,
            Amount = b.Amount,
            BidTime = b.BidTime,
            UserId = b.UserId,
            AuctionId = b.AuctionId,
            BidType = b.BidType,
            MaximumBid = b.MaximumBid,
            IsDelayedBid = b.IsDelayedBid,
            ActivationTime = b.ActivationTime,
        });
    }

    public async Task<BidDTO> GetBidByIdAsync(int id)
    {
        var bid = await _bidRepository.GetBidByIdAsync(id);
        if (bid == null) return null;

        return new BidDTO
        {
            Id = bid.Id,
            Amount = bid.Amount,
            BidTime = bid.BidTime,
            UserId = bid.UserId,
            AuctionId = bid.AuctionId,
            BidType = bid.BidType,
            MaximumBid = bid.MaximumBid,
            IsDelayedBid = bid.IsDelayedBid,
            ActivationTime = bid.ActivationTime,
        };
    }

    public async Task CreateBidAsync(CreateBidDTO bidDto)
    {
        var auction = await _auctionRepository.GetAuctionByIdAsync(bidDto.AuctionId);
        if (auction == null) throw new Exception("Auction not found");
        uint auctionXmin = auction.Xmin;

        // Identify the current highest bid for the auction
        var currentHighestBid = await _auctionRepository.GetCurrentWinningBidForAuction(bidDto.AuctionId);

        // Calculate the minimum bid increment based on the current highest bid
        var minimumBidIncrement = CalculateBidIncrement(currentHighestBid?.WinningBid?.Amount ?? auction.StartingPrice);
        var minimumAllowedBid = (currentHighestBid?.WinningBid?.Amount ?? auction.StartingPrice) + minimumBidIncrement;

        if (bidDto.Amount < minimumAllowedBid) throw new Exception($"Bid must be at least {minimumAllowedBid}");


        // Handle different bid types
        switch (bidDto.BidType)
        {
            case "AuctionBid":
                ExtendAuctionTimeIfNeeded(auction);

                var bid = new Bid
                {
                    Amount = bidDto.Amount,
                    BidTime = bidDto.BidTime,
                    UserId = bidDto.UserId,
                    AuctionId = bidDto.AuctionId,
                    BidType = bidDto.BidType,
                    MaximumBid = bidDto.MaximumBid,
                    IsDelayedBid = bidDto.IsDelayedBid,
                    ActivationTime = bidDto.ActivationTime,
                };
                //_context.Entry(bid).State = EntityState.Modified;
                uint receivedAuctionXmin = auctionXmin;

                // Fetch the current Xmin value of the auction from the database
                Auction currentAuction = await _auctionRepository.GetAuctionByIdAsync(bidDto.AuctionId);

                uint currentAuctionXmin = currentAuction.Xmin;

                // Compare the received Xmin with the current Xmin
                if (receivedAuctionXmin != currentAuctionXmin)
                {
                    // Handle the concurrency conflict (e.g., notify the client, provide options for resolution)
                    throw new CustomConcurrencyException("Concurrency conflict detected. Please refresh and try again.");
                }

                // Continue with bid creation if there's no conflict
                await _bidRepository.CreateBidAsync(bid);

                // Calculate the new bid increment based on the new highest bid
                decimal newBidIncrement = CalculateBidIncrement(bid.Amount);

                // Prepare the message to broadcast the new bid increment
                var bidIncrementMessage = new
                {
                    Type = "BID_INCREMENT_UPDATE",
                    Data = new
                    {
                        AuctionId = bidDto.AuctionId,
                        NewBidIncrement = newBidIncrement
                    }
                };

                // Convert the message to JSON
                var jsonBidIncrementMessage = JsonConvert.SerializeObject(bidIncrementMessage);

                // Broadcast the message to all connected clients
                await _appWebSocketManager.BroadcastMessageAsync(jsonBidIncrementMessage);

                break;
            case "ProxyBid":
                ExtendAuctionTimeIfNeeded(auction);
                // Handle proxy bidding
                await HandleProxyBidAsync(bidDto, currentHighestBid?.WinningBid, minimumBidIncrement);
                break;
            case "DelayedBid":
                ExtendAuctionTimeIfNeeded(auction);
                // Handle delayed bid
                await HandleDelayedBidAsync(bidDto);
                break;
            default:
                throw new Exception("Invalid bid type");
        }
    }
    // Extend auction time if the bid is placed within the last 5 minutes


    //var bid = new Bid
    //{
    //    Amount = bidDto.Amount,
    //    BidTime = bidDto.BidTime,
    //    UserId = bidDto.UserId,
    //    AuctionId = bidDto.AuctionId,
    //    BidType = bidDto.BidType,
    //    MaximumBid = bidDto.MaximumBid,
    //    IsDelayedBid = bidDto.IsDelayedBid,
    //    ActivationTime = bidDto.ActivationTime,
    //};
    //try
    //{
    //    await _bidRepository.CreateBidAsync(bid);
    //}
    //catch (CustomConcurrencyException ex)
    //{
    //    // A concurrency conflict occurred
    //    // Handle the concurrency issue, e.g., by reloading the auction data and retrying or notifying the user
    //    // You might want to log the exception and return a user-friendly message
    //    throw new CustomConcurrencyException("The auction has been modified by another transaction. Please try again.", ex);
    //}

    public async Task UpdateBidAsync(UpdateBidDTO bidDto)
    {
        var existingBid = await _bidRepository.GetBidByIdAsync(bidDto.Id);
        if (existingBid == null) throw new Exception("Bid not found");

        existingBid.Amount = bidDto.Amount ?? existingBid.Amount;
        existingBid.BidTime = bidDto.BidTime ?? existingBid.BidTime;

        await _bidRepository.UpdateBidAsync(existingBid);
    }

    public async Task DeleteBidAsync(int id)
    {
        var bid = await _bidRepository.GetBidByIdAsync(id);
        if (bid == null) throw new Exception("Bid not found");

        await _bidRepository.DeleteBidAsync(bid);
    }

    public decimal CalculateBidIncrement(decimal currentBid)
    {
        if (currentBid <= 24)
        {
            return 1; // Increment by $1 for bids up to $24
        }
        else if (currentBid <= 49)
        {
            return 2; // Increment by $2 for bids $25 - $49
        }
        else if (currentBid <= 74)
        {
            return 3; // Increment by $3 for bids $50 - $74
        }
        else if (currentBid <= 99)
        {
            return 4; // Increment by $4 for bids $75 - $99
        }
        else if (currentBid <= 499)
        {
            return 5; // Increment by $5 for bids $100 - $499
        }
        else if (currentBid <= 999)
        {
            return 10; // Increment by $10 for bids $500 - $999
        }
        else
        {
            return 25; // Increment by $25 for bids $1000 or more
        }
    }


    private async void ExtendAuctionTimeIfNeeded(Auction auction)
    {
        var currentTime = DateTime.UtcNow;
        var timeLeft = auction.EndTime - currentTime;

        if (timeLeft <= TimeSpan.FromMinutes(5))
        {
            auction.EndTime = currentTime.AddMinutes(5);
            await _auctionRepository.UpdateAuctionAsync(auction);
        }
    }

    //private async Task HandleProxyBidAsync(CreateBidDTO bidDto, Bid currentHighestBid, decimal minimumBidIncrement)
    //{
    //    // Fetch all proxy bids for the auction
    //    var proxyBids = await _auctionRepository.GetProxyBidsForAuction(bidDto.AuctionId);

    //    // Sort proxy bids by maximum bid amount in descending order
    //    var sortedProxyBids = proxyBids.OrderByDescending(b => b.MaximumBid).ToList();

    //    // Determine the highest proxy bid (if any)
    //    var highestProxyBid = sortedProxyBids.FirstOrDefault();

    //    // Calculate the next bid amount
    //    decimal nextBidAmount = currentHighestBid?.Amount + minimumBidIncrement ?? bidDto.Amount;

    //    // Check if the new proxy bid has the highest maximum bid
    //    if (bidDto.MaximumBid > (highestProxyBid?.MaximumBid ?? 0))
    //    {
    //        // If so, the next bid should be just higher than the second-highest proxy bid's maximum (or the current highest bid)
    //        var secondHighestProxyBid = sortedProxyBids.Skip(1).FirstOrDefault();
    //        var secondHighestMax = secondHighestProxyBid?.MaximumBid ?? currentHighestBid?.Amount ?? 0;
    //        nextBidAmount = Math.Min(bidDto.MaximumBid.Value, secondHighestMax + minimumBidIncrement);
    //    }

    //    // Ensure the next bid amount does not exceed the new proxy bid's maximum
    //    nextBidAmount = Math.Min(nextBidAmount, bidDto.MaximumBid.Value);

    //    // Create a new bid at the calculated bid amount
    //    var proxyBid = new Bid
    //    {
    //        Amount = nextBidAmount,
    //        BidTime = DateTime.UtcNow,
    //        UserId = bidDto.UserId,
    //        AuctionId = bidDto.AuctionId,
    //        BidType = "ProxyBid",
    //        MaximumBid = bidDto.MaximumBid,
    //        IsDelayedBid = false
    //    };

    //    // Save the proxy bid
    //    await _bidRepository.CreateBidAsync(proxyBid);

    //    // Notify the user based on the bid amount and their maximum bid
    //    if (nextBidAmount < bidDto.MaximumBid)
    //    {
    //        // Notify the user that their proxy bid has been outbid
    //    }
    //    else
    //    {
    //        // Notify the user that they are the current highest bidder
    //    }
    //}

    private async Task HandleProxyBidAsync(CreateBidDTO bidDto, Bid currentHighestBid, decimal minimumBidIncrement)
    {
        // Start a transaction to ensure atomic operations
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                // Fetch and sort all proxy bids for the auction
                var proxyBids = await _auctionRepository.GetProxyBidsForAuction(bidDto.AuctionId);
                var sortedProxyBids = proxyBids.OrderByDescending(b => b.MaximumBid).ToList();

                // Determine the highest and second-highest proxy bids
                var highestProxyBid = sortedProxyBids.FirstOrDefault();
                var secondHighestProxyBid = sortedProxyBids.Skip(1).FirstOrDefault();

                // Calculate the next bid amount based on proxy bid logic
                decimal nextBidAmount = CalculateNextBidAmount(bidDto, currentHighestBid, highestProxyBid, secondHighestProxyBid, minimumBidIncrement);

                // Create and save the new proxy bid
                var proxyBid = CreateProxyBid(bidDto, nextBidAmount);
                await _bidRepository.CreateBidAsync(proxyBid);

                // Commit the transaction
                transaction.Commit();

                // Notify the user based on the bid amount and their maximum bid
                NotifyUserAboutBidStatus(proxyBid, bidDto.MaximumBid.Value);

                // Prepare the message to broadcast the new bid increment
                var newProxyBidMessage = new
                {
                    Type = "NEW_PROXY_BID",
                    Data = new
                    {
                        AuctionId = bidDto.AuctionId,
                        proxyBidUser = proxyBid.UserId,
                        proxyBidMaxAmount = proxyBid.MaximumBid,
                        MaxBid = bidDto.MaximumBid.Value,
                    }
                };

                // Convert the message to JSON
                var jsonNewProxyBidMessage = JsonConvert.SerializeObject(newProxyBidMessage);

                // Broadcast the message to all connected clients
                await _appWebSocketManager.BroadcastMessageAsync(jsonNewProxyBidMessage);
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error
                transaction.Rollback();
                // Handle the exception (log it, notify the user, etc.)
            }
        }
    }

    public async Task ProcessProxyBids()
    {
        var activeAuctions = await _auctionRepository.GetActiveAuctionsAsync();

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                foreach (var auction in activeAuctions)
                {
                    var proxyBids = await _auctionRepository.GetProxyBidsForAuction(auction.Id);
                    if (!proxyBids.Any()) continue;

                    var currentHighestBid = await _auctionRepository.GetCurrentWinningBidForAuction(auction.Id);
                    var sortedProxyBids = proxyBids.OrderByDescending(b => b.MaximumBid).ToList();

                    // Filter out the current highest bidder
                    var eligibleProxyBids = sortedProxyBids.Where(b => currentHighestBid == null || b.UserId != currentHighestBid.WinningBid.UserId).ToList();
                    var highestEligibleProxyBid = eligibleProxyBids.FirstOrDefault();
                    var secondHighestEligibleProxyBid = eligibleProxyBids.Skip(1).FirstOrDefault();

                    if (highestEligibleProxyBid != null &&
                        (currentHighestBid == null || highestEligibleProxyBid.MaximumBid > currentHighestBid.WinningBid.Amount))
                    {
                        var minimumBidIncrement = CalculateBidIncrement(currentHighestBid?.WinningBid?.Amount ?? auction.StartingPrice);

                        var highestEligibleProxyBidDTO = ConvertToCreateBidDTO(highestEligibleProxyBid);
                        decimal nextBidAmount = CalculateNextBidAmount(highestEligibleProxyBidDTO, currentHighestBid?.WinningBid, highestEligibleProxyBid, secondHighestEligibleProxyBid, minimumBidIncrement);

                        if (nextBidAmount <= highestEligibleProxyBid.MaximumBid)
                        {
                            // Place the new bid
                            var newBid = CreateProxyBid(highestEligibleProxyBidDTO, nextBidAmount); // Adapt this method if necessary
                            await _bidRepository.CreateBidAsync(newBid);

                            // Broadcast the new bid
                            var newBidMessage = new
                            {
                                Type = "NEW_BID",
                                Data = new
                                {
                                    AuctionId = auction.Id,
                                    BidAmount = newBid.Amount,
                                    BidType = newBid.BidType,
                                    MaxBidAmount = newBid.MaximumBid,
                                    userWhoBid = newBid.UserId
                                }
                            };
                            var jsonNewBidMessage = JsonConvert.SerializeObject(newBidMessage);
                            await _appWebSocketManager.BroadcastMessageAsync(jsonNewBidMessage);
                            ExtendAuctionTimeIfNeeded(auction);
                        }
                    }
                }
                transaction.Commit();

            }

            catch (Exception ex)
            {
                // If there's an error, rollback the transaction
                transaction.Rollback();
                _logger.LogError(ex, "An error occurred while processing proxy bids.");
                // Handle the exception (log it, notify the user, etc.)

            }
        }

    }


    private CreateBidDTO ConvertToCreateBidDTO(Bid bid)
    {
        return new CreateBidDTO
        {
            Amount = bid.Amount,
            AuctionId = bid.AuctionId,
            UserId = bid.UserId,
            MaximumBid = bid.MaximumBid,
            ActivationTime = bid.ActivationTime,
            // Set other necessary properties from the Bid object
        };
    }



    private decimal CalculateNextBidAmount(CreateBidDTO bidDto, Bid currentHighestBid, Bid highestProxyBid, Bid secondHighestProxyBid, decimal minimumBidIncrement)
    {
        decimal nextBidAmount = currentHighestBid?.Amount + minimumBidIncrement ?? bidDto.Amount;

        if (bidDto.MaximumBid > (highestProxyBid?.MaximumBid ?? 0))
        {
            var secondHighestMax = secondHighestProxyBid?.MaximumBid ?? currentHighestBid?.Amount ?? 0;
            nextBidAmount = Math.Min(bidDto.MaximumBid.Value, secondHighestMax + minimumBidIncrement);
        }

        nextBidAmount = Math.Min(nextBidAmount, bidDto.MaximumBid.Value);
        return nextBidAmount;
    }

    private Bid CreateProxyBid(CreateBidDTO bidDto, decimal bidAmount)
    {
        var proxyBid = new Bid
        {
            Amount = bidAmount,
            BidTime = DateTime.UtcNow,
            UserId = bidDto.UserId,
            AuctionId = bidDto.AuctionId,
            BidType = "ProxyBid",
            MaximumBid = bidDto.MaximumBid,
            IsDelayedBid = false,
            ActivationTime = bidDto.ActivationTime,

        };
        return proxyBid;
    }

    private void NotifyUserAboutBidStatus(Bid proxyBid, decimal maximumBid)
    {
        if (proxyBid.Amount < maximumBid)
        {
            // Notify the user that their proxy bid has been outbid
            // Implement the notification logic (e.g., send an email or WebSocket message)
            Console.WriteLine($"User {proxyBid.UserId} has been outbid. Current bid: {proxyBid.Amount}, Maximum bid: {maximumBid}");
        }
        else
        {
            // Notify the user that they are the current highest bidder
            // Implement the notification logic (e.g., send an email or WebSocket message)
            Console.WriteLine($"User {proxyBid.UserId} is the current highest bidder with a bid of {proxyBid.Amount}");
        }
    }

    private async Task HandleDelayedBidAsync(CreateBidDTO bidDto)
    {
        // Logic to handle delayed bids
        // - Store the bid with the activation time
        // - Ensure it gets activated at the specified time
    }
}
