using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MineralKingdomApi.DTOs.AuctionStatusDTOs;
using MineralKingdomApi.Services;

namespace MineralKingdomApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionStatusController : ControllerBase
    {
        private readonly IAuctionStatusService _auctionStatusService;

        public AuctionStatusController(IAuctionStatusService auctionStatusService)
        {
            _auctionStatusService = auctionStatusService ?? throw new ArgumentNullException(nameof(auctionStatusService));
        }

        // GET: api/AuctionStatus
        [HttpGet]
        public async Task<IActionResult> GetAllAuctionStatuses()
        {
            var auctionStatuses = await _auctionStatusService.GetAllAuctionStatusesAsync();
            return Ok(auctionStatuses);
        }

        // GET: api/AuctionStatus/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuctionStatusById(int id)
        {
            try
            {
                var auctionStatus = await _auctionStatusService.GetAuctionStatusByIdAsync(id);
                return Ok(auctionStatus);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: api/AuctionStatus
        [HttpPost]
        public async Task<IActionResult> CreateAuctionStatus([FromBody] CreateAuctionStatusDTO createDto)
        {
            if (createDto == null)
            {
                return BadRequest();
            }

            var createdAuctionStatus = await _auctionStatusService.CreateAuctionStatusAsync(createDto);
            return CreatedAtAction(nameof(GetAuctionStatusById), new { id = createdAuctionStatus.Id }, createdAuctionStatus);
        }

        // PUT: api/AuctionStatus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuctionStatus(int id, [FromBody] UpdateAuctionStatusDTO updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest();
            }

            try
            {
                var updatedAuctionStatus = await _auctionStatusService.UpdateAuctionStatusAsync(id, updateDto);
                return Ok(updatedAuctionStatus);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // DELETE: api/AuctionStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuctionStatus(int id)
        {
            try
            {
                await _auctionStatusService.DeleteAuctionStatusAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        //[HttpGet("{id}/details")]
        //public async Task<IActionResult> GetAuctionStatusDetails(int id)
        //{
        //    try
        //    {
        //        var auctionStatusDetails = await _auctionStatusService.GetAuctionStatusDetailsByIdAsync(id);
        //        return Ok(auctionStatusDetails);
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception...
        //        return StatusCode(500, "Internal server error" + ex);
        //    }
        //}

    }
}
