using System;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
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
        public async Task<CartItem> GetCartItemByIdAsync(int itemId)
        {
            try
            {
                return await _context.CartItems
                    .Include(ci => ci.Mineral) // Include related Mineral data if needed
                    .FirstOrDefaultAsync(ci => ci.Id == itemId);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error fetching cart item with ID {itemId}.", ex);
            }
        }

        public async Task CreateCartItemAsync(CartItem cartItem)
        {
            if (cartItem == null)
            {
                throw new ArgumentNullException(nameof(cartItem), "Cart item cannot be null.");
            }

            try
            {
                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Error creating cart item.", ex);
            }
        }

        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            if (cartItem == null)
            {
                throw new ArgumentNullException(nameof(cartItem), "Cart item cannot be null.");
            }

            try
            {
                _context.Entry(cartItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error updating cart item with ID {cartItem.Id}.", ex);
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

    }
}

