using System.Threading.Tasks;
using MineralKingdomApi.Models;

public interface ICartItemRepository
{
    Task<CartItem> GetCartItemByIdAsync(int itemId);
    Task CreateCartItemAsync(CartItem cartItem);
    Task UpdateCartItemAsync(CartItem cartItem);
    Task DeleteCartItemAsync(int itemId);
}
