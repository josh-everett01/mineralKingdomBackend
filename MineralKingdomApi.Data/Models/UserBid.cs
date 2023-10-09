using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.Models
{
    public class UserBid
    {
        [Key]
        public int Id { get; set; }

        // Relationship Navigation Property: A UserBid is associated with one User
        public int UserId { get; set; }
        public User? User { get; set; }

        // Relationship Navigation Property: A UserBid is associated with one Bid
        public int BidId { get; set; }
        public Bid? Bid { get; set; }
    }
}
