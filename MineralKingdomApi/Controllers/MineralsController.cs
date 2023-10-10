using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
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

        // Implement other CRUD actions (e.g., POST, PUT, DELETE) as needed.
    }
}