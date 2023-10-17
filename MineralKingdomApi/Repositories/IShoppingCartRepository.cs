using System;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Repositories
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart> GetCartByUserIdAsync(int userId);
        Task<ShoppingCart> GetCartWithItemsByUserIdAsync(int userId);
        Task CreateCartAsync(ShoppingCart cart);
        Task UpdateCartAsync(ShoppingCart cart);
        Task DeleteCartAsync(int cartId);
    }
}

