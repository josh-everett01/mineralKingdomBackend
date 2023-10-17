using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MineralKingdomApi.DTOs.ShoppingCartDTOs;
using MineralKingdomApi.Services;

namespace MineralKingdomApi.Controllers
{
    /// <summary>
    /// Controller for managing shopping carts.
    /// </summary>
    [Route("api/shoppingcart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        /// <summary>
        /// Get a user's shopping cart by user ID.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Shopping cart for the user</returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartByUserId(int userId)
        {
            try
            {
                var cart = await _shoppingCartService.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    return NotFound($"Cart not found for user with ID {userId}");
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a user's shopping cart with items by user ID.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Shopping cart with items for the user</returns>
        [HttpGet("withitems/{userId}")]
        public async Task<IActionResult> GetCartWithItemsByUserId(int userId)
        {
            try
            {
                var cart = await _shoppingCartService.GetCartWithItemsByUserIdAsync(userId);
                if (cart == null)
                {
                    return NotFound($"Cart with items not found for user with ID {userId}");
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Create a shopping cart for a user.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>ActionResult</returns>
        [HttpPost("create/{userId}")]
        public async Task<IActionResult> CreateCartForUser(int userId)
        {
            try
            {
                await _shoppingCartService.CreateCartForUserAsync(userId);
                return Ok("Cart created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Update a user's shopping cart.
        /// </summary>
        /// <param name="cart">ShoppingCartDTO containing updated cart information</param>
        /// <returns>ActionResult</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateCart([FromBody] ShoppingCartDTO cart)
        {
            try
            {
                if (cart == null)
                {
                    return BadRequest("Invalid request data.");
                }

                await _shoppingCartService.UpdateCartAsync(cart);
                return Ok("Cart updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a user's shopping cart by cart ID.
        /// </summary>
        /// <param name="cartId">Cart ID</param>
        /// <returns>ActionResult</returns>
        [HttpDelete("{cartId}")]
        public async Task<IActionResult> DeleteCart(int cartId)
        {
            try
            {
                await _shoppingCartService.DeleteCartAsync(cartId);
                return Ok("Cart deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
