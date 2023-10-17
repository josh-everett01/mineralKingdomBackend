using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        // Relationship Navigation Property: A CartItem is associated with one Mineral
        [Required]
        public int MineralId { get; set; }
        public Mineral Mineral { get; set; }

        // Relationship Navigation Property: A CartItem belongs to one ShoppingCart
        [Required]
        public int ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}


