using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MineralKingdomApi.Data.Models;
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
        public DbSet<CheckoutSessionDetails> CheckoutSessionDetails { get; set; }
        public DbSet<CheckoutSessionRequest> CheckoutSessionRequests { get; set; }
        public DbSet<CheckoutSessionResponse> CheckoutSessionResponses { get; set; }
        public DbSet<CustomerRequest> CustomerRequests { get; set; }
        public DbSet<CustomerResponse> CustomerResponses { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<PaymentDetails> PaymentDetails { get; set; }
        public DbSet<CustomerInquiry> CustomerInquiry { get; set; }
        public DbSet<AdminResponse> AdminResponse { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint for SessionId in CheckoutSessionDetails
            modelBuilder.Entity<CheckoutSessionDetails>()
                .HasIndex(c => c.SessionId)
                .IsUnique();

            // Configuring Address as an owned entity
            modelBuilder.Entity<CustomerRequest>().OwnsOne(c => c.Address);
            //modelBuilder.Entity<CustomerResponse>().OwnsOne(c => c.City);
            //modelBuilder.Entity<CustomerResponse>().OwnsOne(c => c.Country);

            modelBuilder.Entity<LineItem>()
                .HasOne(l => l.CheckoutSessionDetails)
                .WithMany(c => c.LineItems)
                .HasForeignKey(l => l.CheckoutSessionDetailsId);

            modelBuilder.Entity<CheckoutSessionRequest>()
                .Property(e => e.PaymentMethodTypes)
                .HasConversion<string>();

            modelBuilder.Entity<CheckoutSessionRequest>()
                .Property(e => e.Mode)
                .HasConversion<string>();

            modelBuilder.Entity<CheckoutSessionResponse>()
                .HasIndex(r => r.SessionId)
                .IsUnique();

            modelBuilder.Entity<Auction>()
            .Property(a => a.Xmin) // Map the Xmin property
            .HasColumnName("Xmin") // The actual PostgreSQL column name
            .HasColumnType("xid")
            .IsConcurrencyToken(); // Mark it as a concurrency token

        }
    }

}
