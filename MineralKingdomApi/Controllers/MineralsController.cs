using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.DTOs;
using MineralKingdomApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MineralKingdomApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MineralsController : ControllerBase
    {
        private readonly MineralKingdomContext _context;

        public MineralsController(MineralKingdomContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mineral>>> GetMinerals()
        {
            return await _context.Minerals.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Mineral>> CreateMineral([FromBody] CreateMineralDto newMineral)
        {
            if (newMineral == null)
            {
                return BadRequest("Mineral data is required");
            }

            // Map DTO to Entity
            var mineral = new Mineral
            {
                Name = newMineral.Name,
                Description = newMineral.Description,
                Price = (decimal)newMineral.Price,
                Origin = newMineral.Origin,
                ImageURL = newMineral.ImageURL,
                CreatedAt = DateTime.UtcNow  // Assuming you have a CreatedAt property
            };

            // Add to DbContext
            await _context.Minerals.AddAsync(mineral);

            // Save Changes
            await _context.SaveChangesAsync();

            // Return the created mineral
            return CreatedAtAction(nameof(GetMineral), new { id = mineral.Id }, mineral);
        }

        // Assume you have a GET method to retrieve a mineral by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Mineral>> GetMineral(int id)
        {
            var mineral = await _context.Minerals.FindAsync(id);
            if (mineral == null)
            {
                return NotFound();
            }
            return mineral;
        }
    }
}