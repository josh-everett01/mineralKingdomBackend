using System;
namespace MineralKingdomApi.DTOs.ShoppingCartDTOs
{
    public class ShoppingCartDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemDTO> CartItems { get; set; }
    }
}

