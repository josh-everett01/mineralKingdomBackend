using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MineralKingdomApi.DTOs.BidDTOs;
using MineralKingdomApi.Services;


/// <summary>
/// Provides API endpoints for managing bids.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BidController : ControllerBase
{
    private readonly IBidService _bidService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BidController"/> class.
    /// </summary>
    /// <param name="bidService">The bid service.</param>
    public BidController(IBidService bidService)
    {
        _bidService = bidService;
    }

    /// <summary>
    /// Gets all bids.
    /// </summary>
    /// <returns>A list of bids.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BidDTO>>> GetAllBids()
    {
        try
        {
            var bids = await _bidService.GetAllBidsAsync();
            return Ok(bids);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a bid by its ID.
    /// </summary>
    /// <param name="id">The bid ID.</param>
    /// <returns>The bid with the specified ID.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<BidDTO>> GetBidById(int id)
    {
        try
        {
            var bid = await _bidService.GetBidByIdAsync(id);
            if (bid == null)
            {
                return NotFound($"Bid with ID {id} not found.");
            }
            return Ok(bid);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates a new bid.
    /// </summary>
    /// <param name="bidDto">The bid data transfer object.</param>
    /// <returns>The created bid.</returns>
    [HttpPost]
    public async Task<ActionResult> CreateBid([FromBody] CreateBidDTO bidDto)
    {
        try
        {
            await _bidService.CreateBidAsync(bidDto);
            return CreatedAtAction(nameof(GetBidById), new { id = bidDto.AuctionId }, bidDto);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates an existing bid.
    /// </summary>
    /// <param name="id">The bid ID.</param>
    /// <param name="bidDto">The bid data transfer object.</param>
    /// <returns>An action result.</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateBid(int id, [FromBody] UpdateBidDTO bidDto)
    {
        if (id != bidDto.Id)
        {
            return BadRequest("Bid ID mismatch.");
        }

        try
        {
            await _bidService.UpdateBidAsync(bidDto);
            return NoContent();
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes a bid by its ID.
    /// </summary>
    /// <param name="id">The bid ID.</param>
    /// <returns>An action result.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBid(int id)
    {
        try
        {
            var bid = await _bidService.GetBidByIdAsync(id);
            if (bid == null)
            {
                return NotFound($"Bid with ID {id} not found.");
            }

            await _bidService.DeleteBidAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
