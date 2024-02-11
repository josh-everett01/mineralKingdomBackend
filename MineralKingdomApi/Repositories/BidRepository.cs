using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.Exceptions;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;

public class BidRepository : IBidRepository
{
    private readonly MineralKingdomContext _context;

    public BidRepository(MineralKingdomContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Bid>> GetAllBidsAsync()
    {
        try
        {
            return await _context.Bids.ToListAsync();
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception("Error fetching all bids.", ex);
        }
    }

    public async Task<Bid> GetBidByIdAsync(int id)
    {
        try
        {
            var bid = await _context.Bids.FindAsync(id);
            if (bid == null)
            {
                throw new Exception($"No bid found with ID {id}.");
            }
            return bid;
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception($"Error fetching bid with ID {id}.", ex);
        }
    }

    public async Task CreateBidAsync(Bid bid)
    {
        if (bid == null)
        {
            throw new ArgumentNullException(nameof(bid), "Bid cannot be null.");
        }

        try
        {
            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Handle or log the concurrency exception
            // You might want to throw a custom exception that the service layer knows how to handle
            throw new CustomConcurrencyException("The auction has been modified by another transaction. Please try again.", ex);
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception("Error creating bid.", ex);
        }
    }

    public async Task UpdateBidAsync(Bid bid)
    {
        if (bid == null)
        {
            throw new ArgumentNullException(nameof(bid), "Bid cannot be null.");
        }

        try
        {
            _context.Entry(bid).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception($"Error updating bid with ID {bid.Id}.", ex);
        }
    }

    public async Task DeleteBidAsync(Bid bid)
    {
        if (bid == null)
        {
            throw new ArgumentNullException(nameof(bid), "Bid cannot be null.");
        }

        try
        {
            _context.Bids.Remove(bid);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception($"Error deleting bid with ID {bid.Id}.", ex);
        }
    }
}
