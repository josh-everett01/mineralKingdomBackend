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
        private readonly ILogger<CartItemRepository> _logger;
        private readonly IMineralRepository _mineralRepository;


        public CartItemRepository(MineralKingdomContext context, ILogger<CartItemRepository> logger, IMineralRepository mineralRepository)
        {
            _context = context;
            _logger = logger;
            _mineralRepository = mineralRepository;
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

        public async Task CreateCartItemAsync(int userId, CartItemDTO cartItemDTO, int cartId)
        {
            if (cartItemDTO == null)
            {
                throw new ArgumentNullException(nameof(cartItemDTO), "Cart item DTO cannot be null.");
            }

            try
            {
                var cartItem = MapDTOToCartItem(cartItemDTO);
                cartItem.ShoppingCartId = cartId;
                cartItem.MineralId = cartItemDTO.MineralId;
                cartItem.Mineral = await _mineralRepository.GetMineralByIdAsync(cartItemDTO.MineralId);

                if (cartItem.Mineral.IsAuctionItem)
                {
                    // Check if the auction winner has been notified
                }

                _logger.LogInformation($"context.CartItems: " + _context.CartItems);
                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cart item. Exception: {Exception}", ex.ToString());
                throw;
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

        public async Task DeleteCartItemAsync(CartItem cartItem)
        {
            try
            {
                var cartItemMatch = await _context.CartItems.FindAsync(cartItem.Id);
                if (cartItem == null)
                {
                    throw new Exception($"No cart item found with ID {cartItem.Id}.");
                }

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error deleting cart item with ID {cartItem.Id}.", ex);
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
                MineralId = cartItemDTO.MineralId
            };
        }

    }
}

