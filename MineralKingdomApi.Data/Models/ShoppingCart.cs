using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        // Relationship Navigation Property: A ShoppingCart is associated with one User
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        // Navigation property for the items in the cart
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}


