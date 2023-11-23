using System;
using System.Linq;
using System.Threading.Tasks;
using MineralKingdomApi.DTOs.ShoppingCartDTOs;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly ICartItemRepository _cartItemRepository;

    public ShoppingCartService(IShoppingCartRepository shoppingCartRepository, ICartItemRepository cartItemRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _cartItemRepository = cartItemRepository;
    }

    public async Task<ShoppingCartDTO?> GetCartByUserIdAsync(int userId)
    {
        try
        {
            var shoppingCart = await _shoppingCartRepository.GetCartByUserIdAsync(userId);
            if (shoppingCart == null)
            {
                return null;
            }

            return MapShoppingCartToDTO(shoppingCart);
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception("Error fetching cart by user ID.", ex);
        }
    }

    public async Task<ShoppingCartDTO> GetCartWithItemsByUserIdAsync(int userId)
    {
        try
        {
            var shoppingCart = await _shoppingCartRepository.GetCartWithItemsByUserIdAsync(userId);
            return MapShoppingCartToDTO(shoppingCart);
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception("Error fetching cart with items by user ID.", ex);
        }
    }

    public async Task CreateCartForUserAsync(int userId)
    {
        try
        {
            var existingCart = await _shoppingCartRepository.GetCartByUserIdAsync(userId);
            if (existingCart != null)
            {
                throw new Exception("User already has a cart.");
            }

            var newCart = new ShoppingCart { UserId = userId };
            await _shoppingCartRepository.CreateCartAsync(newCart);
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception("Error creating cart for the user.", ex);
        }
    }

    public async Task UpdateCartAsync(ShoppingCartDTO cart)
    {
        if (cart == null)
        {
            throw new ArgumentNullException(nameof(cart), "Cart cannot be null.");
        }

        try
        {
            var existingCart = await _shoppingCartRepository.GetCartByUserIdAsync(cart.UserId);
            if (existingCart == null)
            {
                throw new Exception($"Cart with user ID {cart.UserId} does not exist.");
            }

            // Update cart properties as needed
            existingCart.CartItems = cart.CartItems?.Select(item => MapCartItemToEntity(item)).ToList();

            await _shoppingCartRepository.UpdateCartAsync(existingCart);
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception($"Error updating cart for user ID {cart.UserId}.", ex);
        }
    }

    public async Task DeleteCartAsync(int cartId)
    {
        try
        {
            await _shoppingCartRepository.DeleteCartAsync(cartId);
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception($"Error deleting cart with ID {cartId}.", ex);
        }
    }

        public async Task AddItemToCartAsync(int userId, CartItemDTO cartItemDTO)
        {
            try
            {
                var cart = await _shoppingCartRepository.GetCartWithItemsByUserIdAsync(userId);
                if (cart == null)
                {
                    // If the cart doesn't exist, create it
                    cart = new ShoppingCart { UserId = userId };
                    await _shoppingCartRepository.CreateCartAsync(cart);
                }

                // Check if the item is already in the cart
                if (cart.CartItems.Any(ci => ci.MineralId == cartItemDTO.MineralId))
                {
                    throw new InvalidOperationException("Item is already in the cart.");
                }
     
                // Add the item to the cart
                await _cartItemRepository.CreateCartItemAsync(userId, cartItemDTO, cart.Id);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Error adding item to cart.", ex);
            }
        }

        public async Task RemoveItemFromCartAsync(int userId, int cartItemId)
        {
            try
            {
                var cart = await _shoppingCartRepository.GetCartWithItemsByUserIdAsync(userId);
                if (cart == null)
                {
                    throw new InvalidOperationException("Shopping cart not found.");
                }

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.MineralId == cartItemId);
                if (cartItem == null)
                {
                    throw new InvalidOperationException("Item not found in the cart.");
                }

                await _cartItemRepository.DeleteCartItemAsync(cartItem);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Error removing item from cart.", ex);
            }
        }

    // Helper methods for mapping between DTOs and entities
    private ShoppingCartDTO MapShoppingCartToDTO(ShoppingCart shoppingCart)
    {
        return new ShoppingCartDTO
        {
            Id = shoppingCart.Id,
            UserId = shoppingCart.UserId,
            CartItems = shoppingCart.CartItems?.Select(item => MapCartItemToDTO(item)).ToList()
        };
    }

    private CartItemDTO MapCartItemToDTO(CartItem cartItem)
    {
        return new CartItemDTO
        {
            Id = cartItem.Id,
            MineralId = cartItem.MineralId
        };
    }

    private CartItem MapCartItemToEntity(CartItemDTO cartItemDTO)
    {
        return new CartItem
        {
            Id = cartItemDTO.Id,
            MineralId = cartItemDTO.MineralId
        };
    }
}
