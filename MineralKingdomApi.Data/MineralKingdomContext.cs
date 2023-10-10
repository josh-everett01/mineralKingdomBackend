using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Data
{
    public class MineralKingdomContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public MineralKingdomContext(DbContextOptions<MineralKingdomContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

        }

        // Define DbSet properties for your entities here
        public DbSet<Mineral> Minerals { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<AuctionStatus> AuctionStatuses { get; set; }
        public DbSet<Bid> Bids { get; set; }
    }

}
