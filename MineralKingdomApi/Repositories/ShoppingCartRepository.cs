using System;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly MineralKingdomContext _context;

        public ShoppingCartRepository(MineralKingdomContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCart> GetCartByUserIdAsync(int userId)
        {
            try
            {
                return await _context.ShoppingCarts
                    .FirstOrDefaultAsync(sc => sc.UserId == userId);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Error fetching cart by user ID.", ex);
            }
        }

        public async Task<ShoppingCart> GetCartWithItemsByUserIdAsync(int userId)
        {
            try
            {
                return await _context.ShoppingCarts
                    .Include(sc => sc.CartItems)
                    .ThenInclude(ci => ci.Mineral)
                    .FirstOrDefaultAsync(sc => sc.UserId == userId);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Error fetching cart with items by user ID.", ex);
            }
        }

        public async Task CreateCartAsync(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart), "Cart cannot be null.");
            }

            try
            {
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Error creating cart.", ex);
            }
        }

        public async Task UpdateCartAsync(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart), "Cart cannot be null.");
            }

            try
            {
                _context.Entry(cart).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error updating cart with ID {cart.Id}.", ex);
            }
        }

        public async Task DeleteCartAsync(int cartId)
        {
            try
            {
                var cart = await _context.ShoppingCarts.FindAsync(cartId);
                if (cart == null)
                {
                    throw new Exception($"No cart found with ID {cartId}.");
                }

                _context.ShoppingCarts.Remove(cart);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error deleting cart with ID {cartId}.", ex);
            }
        }
    }
}

