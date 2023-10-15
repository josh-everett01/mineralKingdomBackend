using System;
using Microsoft.AspNetCore.Mvc;
using MineralKingdomApi.DTOs.AuctionDTOs;
using MineralKingdomApi.Models;
using MineralKingdomApi.Services;

namespace MineralKingdomApi.Controllers
{
    /// <summary>
    /// Provides endpoints for managing auctions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionService _auctionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuctionController"/> class.
        /// </summary>
        /// <param name="auctionService">The service used for auction-related operations.</param>
        public AuctionController(IAuctionService auctionService)
        {
            _auctionService = auctionService ?? throw new ArgumentNullException(nameof(auctionService));
        }

        /// <summary>
        /// Retrieves all auctions.
        /// </summary>
        /// <returns>A list of all auctions.</returns>
        /// <response code="200">Returns the list of auctions.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAuctions()
        {
            var auctions = await _auctionService.GetAllAuctionsAsync();
            return Ok(auctions);
        }

        /// <summary>
        /// Retrieves an auction by its ID.
        /// </summary>
        /// <param name="id">The ID of the auction.</param>
        /// <returns>The auction with the specified ID.</returns>
        /// <response code="200">Returns the auction.</response>
        /// <response code="404">If the auction is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuctionById(int id)
        {
            var auction = await _auctionService.GetAuctionByIdAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            return Ok(auction);
        }

        /// <summary>
        /// Creates a new auction.
        /// </summary>
        /// <param name="auction">The auction to create.</param>
        /// <returns>A newly created auction.</returns>
        /// <response code="201">Returns the newly created auction.</response>
        /// <response code="400">If the auction is null.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionDTO createAuctionDto)
        {
            if (createAuctionDto == null)
            {
                return BadRequest();
            }

            // Map DTO to Domain Model
            var auction = new Auction
            {
                Title = createAuctionDto.Title,
                Description = createAuctionDto.Description,
                StartingPrice = createAuctionDto.StartingPrice,
                StartTime = createAuctionDto.StartTime,
                EndTime = createAuctionDto.EndTime,
                MineralId = createAuctionDto.MineralId,
                AuctionStatusId = createAuctionDto.AuctionStatusId
            };

            await _auctionService.CreateAuctionAsync(auction);
            return CreatedAtAction(nameof(GetAuctionById), new { id = auction.Id }, auction);
        }

        /// <summary>
        /// Updates an existing auction.
        /// </summary>
        /// <param name="id">The ID of the auction to update.</param>
        /// <param name="auction">The updated auction.</param>
        /// <returns>No content.</returns>
        /// <response code="204">If the auction is updated successfully.</response>
        /// <response code="400">If the ID does not match the auction's ID.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAuction(int id, Auction auction)
        {
            // Check if auction is null
            if (auction == null)
            {
                return BadRequest("Auction cannot be null");
            }

            // Check if id does not match auction.Id
            if (id != auction.Id)
            {
                return BadRequest("Mismatched auction ID");
            }

            await _auctionService.UpdateAuctionAsync(auction);
            return NoContent();
        }

        /// <summary>
        /// Deletes an auction.
        /// </summary>
        /// <param name="id">The ID of the auction to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">If the auction is deleted successfully.</response>
        /// <response code="404">If the auction is not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAuction(int id)
        {
            var auction = await _auctionService.GetAuctionByIdAsync(id);
            if (auction == null)
            {
                return NotFound();
            }

            await _auctionService.DeleteAuctionAsync(auction);
            return NoContent();
        }

        /// <summary>
        /// Retrieves all active auctions.
        /// </summary>
        /// <returns>A list of all active auctions.</returns>
        /// <response code="200">Returns the list of active auctions.</response>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveAuctions()
        {
            var auctions = await _auctionService.GetActiveAuctionsAsync();
            return Ok(auctions);
        }

        /// <summary>
        /// Retrieves auctions by status.
        /// </summary>
        /// <param name="statusId">The ID of the auction status.</param>
        /// <returns>A list of auctions with the specified status.</returns>
        /// <response code="200">Returns the list of auctions.</response>
        [HttpGet("status/{statusId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuctionsByStatus(int statusId)
        {
            var auctions = await _auctionService.GetAuctionsByStatusAsync(statusId);
            return Ok(auctions);
        }

        /// <summary>
        /// Retrieves auctions by user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of auctions created by the specified user.</returns>
        /// <response code="200">Returns the list of auctions.</response>
        //[HttpGet("user/{userId}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetAuctionsByUser(int userId)
        //{
        //    var auctions = await _auctionService.GetAuctionsByUserAsync(userId);
        //    return Ok(auctions);
        //}

        /// <summary>
        /// Retrieves auctions with bids.
        /// </summary>
        /// <returns>A list of auctions that have received bids.</returns>
        /// <response code="200">Returns the list of auctions.</response>
        [HttpGet("with-bids")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuctionsWithBids()
        {
            var auctions = await _auctionService.GetAuctionsWithBidsAsync();
            return Ok(auctions);
        }

        /// <summary>
        /// Retrieves the winning bid for an auction.
        /// </summary>
        /// <param name="auctionId">The ID of the auction.</param>
        /// <returns>The winning bid for the specified auction.</returns>
        /// <response code="200">Returns the winning bid.</response>
        /// <response code="404">If the auction or bid is not found.</response>
        [HttpGet("{auctionId}/winning-bid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWinningBidForAuction(int auctionId)
        {
            var bid = await _auctionService.GetCurrentWinningBidForAuction(auctionId);
            if (bid == null)
            {
                return NotFound();
            }
            return Ok(bid);
        }

        /// <summary>
        /// Retrieves bids for an auction.
        /// </summary>
        /// <param name="auctionId">The ID of the auction.</param>
        /// <returns>A list of bids for the specified auction.</returns>
        /// <response code="200">Returns the list of bids.</response>
        [HttpGet("{auctionId}/bids")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBidsForAuction(int auctionId)
        {
            var bids = await _auctionService.GetBidsForAuctionAsync(auctionId);
            return Ok(bids);
        }

        /// <summary>
        /// Retrieves auctions for a mineral.
        /// </summary>
        /// <param name="mineralId">The ID of the mineral.</param>
        /// <returns>A list of auctions for the specified mineral.</returns>
        /// <response code="200">Returns the list of auctions.</response>
        [HttpGet("mineral/{mineralId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAuctionsForMineral(int mineralId)
        {
            var auctions = await _auctionService.GetAuctionsForMineralAsync(mineralId);
            return Ok(auctions);
        }

        /// <summary>
        /// Retrieves the winning bid for a completed auction.
        /// </summary>
        /// <param name="auctionId">The ID of the auction.</param>
        /// <returns>The winning bid for the completed auction.</returns>
        /// <response code="200">Returns the winning bid.</response>
        /// <response code="404">If the auction or bid is not found.</response>
        [HttpGet("{auctionId}/completed/winning-bid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWinningBidForCompletedAuction(int auctionId)
        {
            var bidResult = await _auctionService.GetWinningBidForCompletedAuction(auctionId);
            if (bidResult == null)
            {
                return NotFound();
            }
            return Ok(bidResult);
        }
    }
}

