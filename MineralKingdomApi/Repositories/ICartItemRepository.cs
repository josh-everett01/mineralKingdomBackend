using System.Threading.Tasks;
using MineralKingdomApi.DTOs.ShoppingCartDTOs;
using MineralKingdomApi.Models;

public interface ICartItemRepository
{
    Task<CartItemDTO> GetCartItemByIdAsync(int itemId);
    Task CreateCartItemAsync(int userId, CartItemDTO cartItemDTO, int cartId);
    Task UpdateCartItemAsync(CartItemDTO cartItemDTO);
    Task DeleteCartItemAsync(CartItem cartItem);
}
