using System;
using MineralKingdomApi.DTOs.AuctionDTOs.YourNamespace.Models;
using MineralKingdomApi.Models;
using MineralKingdomApi.Data;
using Microsoft.EntityFrameworkCore;

namespace MineralKingdomApi.Repositories
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly MineralKingdomContext _context;

        public AuctionRepository(MineralKingdomContext context)
        {
            _context = context;
        }

        public async Task CreateAuctionAsync(Auction auction)
        {
            if (auction == null)
            {
                throw new ArgumentNullException(nameof(auction));
            }
            await _context.Auctions.AddAsync(auction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAuctionAsync(Auction auction)
        {
            if (auction == null)
            {
                throw new ArgumentNullException(nameof(auction));
            }
            _context.Auctions.Remove(auction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Auction>> GetAllAuctionsAsync()
        {
            return await _context.Auctions
                .Include(a => a.AuctionStatus)
                .Include(a => a.Mineral)
                .Include(a => a.Bids)
                .ToListAsync();
        }

        public async Task<Auction?> GetAuctionByIdAsync(int id)
        {
            return await _context.Auctions
                .Include(a => a.AuctionStatus)
                .Include(a => a.Mineral)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Auction>> GetActiveAuctionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Auctions
                .Include(a => a.AuctionStatus)
                .Include(a => a.Mineral)
                .Where(a => a.StartTime <= now && a.EndTime >= now)
                .ToListAsync();
        }

        public async Task<IEnumerable<Auction>> GetAuctionsByStatusAsync(int auctionStatusId)
        {
            return await _context.Auctions
                .Include(a => a.AuctionStatus)
                .Include(a => a.Mineral)
                .Where(a => a.AuctionStatusId == auctionStatusId)
                .ToListAsync();
        }

        //public async Task<IEnumerable<Auction>> GetAuctionsByUserAsync(int userId)
        //{
        //    // Assuming there is a UserId property in your Auction model
        //    return await _context.Auctions
        //        .Include(a => a.AuctionStatus)
        //        .Include(a => a.Mineral)
        //        .Where(a => a.UserId == userId)
        //        .ToListAsync();
        //}

        public async Task<IEnumerable<Auction>> GetAuctionsWithBidsAsync()
        {
            return await _context.Auctions
                .Include(a => a.AuctionStatus)
                .Include(a => a.Mineral)
                .Include(a => a.Bids)
                .ToListAsync();
        }

        public async Task<IEnumerable<Auction>> GetAuctionsForMineralAsync(int mineralId)
        {
            return await _context.Auctions
                .Include(a => a.AuctionStatus)
                .Include(a => a.Mineral)
                .Where(a => a.MineralId == mineralId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bid>> GetBidsForAuctionAsync(int auctionId)
        {
            return await _context.Bids
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.Amount)
                .ToListAsync();
        }

        public async Task<BidResult> GetCurrentWinningBidForAuction(int auctionId)
        {
            var winningBid = await _context.Bids
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.Amount)
                .FirstOrDefaultAsync();

            return new BidResult
            {
                WinningBid = winningBid,
                Message = winningBid != null ? "Current winning bid retrieved successfully." : "No bids available for this auction.",
                IsSuccess = winningBid != null
            };
        }

        public async Task<BidResult> GetWinningBidForCompletedAuction(int auctionId)
        {
            var auction = await _context.Auctions.FindAsync(auctionId);

            // Check if auction is null
            if (auction == null)
            {
                return new BidResult
                {
                    WinningBid = null,
                    Message = "Auction not found.",
                    IsSuccess = false
                };
            }

            if (auction.EndTime > DateTime.UtcNow)
            {
                return new BidResult
                {
                    WinningBid = null,
                    Message = "Auction is still in progress.",
                    IsSuccess = false
                };
            }

            var winningBid = await _context.Bids
                .Where(b => b.AuctionId == auctionId)
                .OrderByDescending(b => b.Amount)
                .FirstOrDefaultAsync();

            return new BidResult
            {
                WinningBid = winningBid,
                Message = winningBid != null ? "Winning bid retrieved successfully." : "No bids available for this auction.",
                IsSuccess = winningBid != null
            };
        }

        public async Task UpdateAuctionAsync(Auction auction)
        {
            if (auction == null)
            {
                throw new ArgumentNullException(nameof(auction));
            }
            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Auction>> GetFinishedAndUnnotifiedAuctionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Auctions
                .Include(a => a.AuctionStatus)
                .Include(a => a.Mineral)
                .Include(a => a.Bids)
                .Where(a => a.EndTime < now && !a.IsWinnerNotified)
                .ToListAsync();
        }

    }
}     
   

