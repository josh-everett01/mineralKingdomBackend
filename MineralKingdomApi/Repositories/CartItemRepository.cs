using System;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.DTOs.ShoppingCartDTOs;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly MineralKingdomContext _context;

        public CartItemRepository(MineralKingdomContext context)
        {
            _context = context;
        }

        public async Task<CartItemDTO> GetCartItemByIdAsync(int itemId)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .Include(ci => ci.Mineral) // Include related Mineral data if needed
                    .FirstOrDefaultAsync(ci => ci.Id == itemId);

                return MapCartItemToDTO(cartItem);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error fetching cart item with ID {itemId}.", ex);
            }
        }

        public async Task CreateCartItemAsync(CartItemDTO cartItemDTO)
        {
            if (cartItemDTO == null)
            {
                throw new ArgumentNullException(nameof(cartItemDTO), "Cart item DTO cannot be null.");
            }

            try
            {
                var cartItem = MapDTOToCartItem(cartItemDTO);
                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Error creating cart item.", ex);
            }
        }

        public async Task UpdateCartItemAsync(CartItemDTO cartItemDTO)
        {
            if (cartItemDTO == null)
            {
                throw new ArgumentNullException(nameof(cartItemDTO), "Cart item DTO cannot be null.");
            }

            try
            {
                var cartItem = MapDTOToCartItem(cartItemDTO);
                _context.Entry(cartItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error updating cart item with ID {cartItemDTO.Id}.", ex);
            }
        }

        public async Task DeleteCartItemAsync(int itemId)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(itemId);
                if (cartItem == null)
                {
                    throw new Exception($"No cart item found with ID {itemId}.");
                }

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error deleting cart item with ID {itemId}.", ex);
            }
        }

        private CartItemDTO MapCartItemToDTO(CartItem cartItem)
        {
            if (cartItem == null) return null;

            return new CartItemDTO
            {
                Id = cartItem.Id,
                MineralId = cartItem.MineralId
            };
        }

        private CartItem MapDTOToCartItem(CartItemDTO cartItemDTO)
        {
            if (cartItemDTO == null) return null;

            return new CartItem
            {
                Id = cartItemDTO.Id,
                MineralId = cartItemDTO.MineralId
            };
        }

    }
}

