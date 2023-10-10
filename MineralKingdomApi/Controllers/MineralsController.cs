using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.DTOs;
using MineralKingdomApi.Models;
using MineralKingdomApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MineralKingdomApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MineralsController : ControllerBase
    {
        private readonly IMineralService _mineralService;

        public MineralsController(IMineralService mineralService)
        {
            _mineralService = mineralService;
        }

        /// <summary>
        /// Retrieves a list of all minerals.
        /// </summary>
        /// <response code="200">List of minerals.</response>
        /// <response code="500">If there is a server error.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] // Indicates that the minerals are returned successfully
        public async Task<ActionResult<IEnumerable<MineralResponseDTO>>> GetMinerals()
        {
            return Ok(await _mineralService.GetAllMineralsAsync());
        }

        /// <summary>
        /// Creates a mineral / Adds it to the database
        /// </summary>
        /// <response code="201">Returns the created mineral.</response>
        /// <response code="400">If there is a server error.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // Indicates that the mineral is created successfully
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Indicates that the request is invalid
        public async Task<ActionResult<MineralResponseDTO>> CreateMineral([FromBody] CreateMineralDTO newMineral)
        {
            if (newMineral == null)
            {
                return BadRequest("Mineral data is required");
            }

            var mineral = await _mineralService.CreateMineralAsync(newMineral);

            var mineralResponseDTO = _mineralService.ConvertToMineralResponseDTO(mineral);

            // Return the created mineral
            return CreatedAtAction(nameof(GetMineral), new { id = mineral.Id }, mineralResponseDTO);
        }

        /// <summary>
        /// Gets a mineral by ID
        /// </summary>
        /// <response code="200">Returns the mineral with the supplied ID.</response>
        /// <response code="404">If the mineral with the supplied ID is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Indicates that the mineral is returned successfully
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Indicates that the mineral is not found
        public async Task<ActionResult<MineralResponseDTO>> GetMineral(int id)
        {
            var mineral = await _mineralService.GetMineralByIdAsync(id);
            if (mineral == null)
            {
                return NotFound();
            }
            return mineral;
        }

        /// <summary>
        /// Updates a mineral.
        /// </summary>
        /// <response code="204">Updated successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If the mineral is not found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Indicates that the mineral is updated successfully
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Indicates that the request is invalid
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Indicates that the mineral is not found
        public async Task<IActionResult> UpdateMineral(int id, [FromBody] UpdateMineralDTO updateMineralDTO)
        {
            var mineral = await _mineralService.GetMineralByIdAsync(id);
            if (mineral == null)
            {
                return NotFound();
            }

            // Update properties
            mineral.Name = updateMineralDTO.Name ?? mineral.Name;
            mineral.Description = updateMineralDTO.Description ?? mineral.Description;
            mineral.Price = updateMineralDTO.Price ?? mineral.Price;
            mineral.Origin = updateMineralDTO.Origin ?? mineral.Origin;

            // Save changes
            await _mineralService.SaveAsync();

            return NoContent(); // 204 No Content response
        }

        /// <summary>
        /// Deletes a mineral.
        /// </summary>
        /// <response code="204">If the mineral was deleted successfully.</response>
        /// <response code="404">If mineral is not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Indicates that the mineral is deleted successfully
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Indicates that the mineral is not found
        public async Task<IActionResult> DeleteMineral(int id)
        {
            var mineral = await _mineralService.GetMineralByIdAsync(id);
            if (mineral == null)
            {
                return NotFound();
            }

            await _mineralService.DeleteMineralAsync(id);
            await _mineralService.SaveAsync();

            return NoContent(); // 204 No Content response
        }
    }
}