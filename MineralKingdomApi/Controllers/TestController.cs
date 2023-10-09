using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly MineralKingdomContext _context;

    public TestController(MineralKingdomContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult TestDatabaseConnection()
    {
        // Query a sample table or entity
        var data = _context.Minerals.FirstOrDefault();

        if (data != null)
        {
            return Ok("Database connection successful!");
        }
        else
        {
            return BadRequest("Unable to retrieve data from the database.");
        }
    }
}
