using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Data
{
    public class MineralKingdomContext : DbContext
    {

        public MineralKingdomContext(DbContextOptions<MineralKingdomContext> options)
            : base(options)
        {
        }

        // Define DbSet properties for your entities here
        public DbSet<Mineral> Minerals { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<AuctionStatus> AuctionStatuses { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
    }

}
