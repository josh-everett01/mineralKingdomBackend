using System;
using MineralKingdomApi.DTOs.ShoppingCartDTOs;
using MineralKingdomApi.Models;

public interface IShoppingCartService
{
    Task<ShoppingCartDTO> GetCartByUserIdAsync(int userId);
    Task<ShoppingCartDTO> GetCartWithItemsByUserIdAsync(int userId);
    Task CreateCartForUserAsync(int userId);
    Task UpdateCartAsync(ShoppingCartDTO cart);
    Task DeleteCartAsync(int cartId);
}


