using System.Threading.Tasks;
using MineralKingdomApi.DTOs.ShoppingCartDTOs;
using MineralKingdomApi.Models;

public interface ICartItemRepository
{
    Task<CartItemDTO> GetCartItemByIdAsync(int itemId);
    Task CreateCartItemAsync(CartItemDTO cartItemDTO);
    Task UpdateCartItemAsync(CartItemDTO cartItemDTO);
    Task DeleteCartItemAsync(int itemId);
}
