﻿using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MineralKingdomApi.DTOs.UserDTOs;
using MineralKingdomApi.DTOs.UserDTOs.MineralKingdomApi.DTOs.UserDTOs;
using MineralKingdomApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MineralKingdomApi.Services.UserService;

namespace MineralKingdomApi.Controllers
{
    /// <summary>
    /// UserController manages user-related operations such as registration, login, and email verification.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                var userResponse = await _userService.RegisterUserAsync(registerDTO);
                return Ok(userResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var userResponse = await _userService.LoginUserAsync(loginDTO);
            if (userResponse != null)
            {
                return Ok(userResponse);
            }
            return Unauthorized("Invalid username or password.");
        }

        /// <summary>
        /// Sends a verification email to the user.
        /// </summary>
        [HttpPost("send-verification-email")]
        public async Task<IActionResult> SendVerificationEmail([FromBody] string email)
        {
            var result = await _userService.ResendVerificationEmail(email);
            if (result)
            {
                return Ok("Verification email sent successfully.");
            }
            return BadRequest("Failed to send verification email.");
        }

        /// <summary>
        /// Verifies a user's email address.
        /// </summary>
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] string token)
        {
            EmailVerificationResult result = await _userService.VerifyUserEmail(token);

            switch (result)
            {
                case EmailVerificationResult.Success:
                    return Ok("Email verified successfully.");
                case EmailVerificationResult.TokenExpired:
                    return BadRequest("Email verification link has expired. Please request a new one.");
                case EmailVerificationResult.TokenInvalid:
                    return BadRequest("Invalid email verification link.");
                case EmailVerificationResult.Error:
                    return StatusCode(500, "An error occurred while verifying your email.");
                default:
                    return BadRequest("Invalid email verification link.");
            }
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound("User not found.");
        }

        /// <summary>
        /// Updates a user's details.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO updateUserDTO)
        {
            var updatedUser = await _userService.UpdateUserAsync(id, updateUserDTO);
            if (updatedUser != null)
            {
                return Ok(updatedUser);
            }
            return NotFound("User not found.");
        }

        /// <summary>
        /// Partially updates a user's information specified by the given ID using a JSON Patch document.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="patchDocument">A JSON Patch document containing partial updates for the user.</param>
        /// <returns>
        /// Returns an IActionResult representing the result of the update operation.
        /// - If the user is updated successfully, it returns an HTTP 200 OK response with the updated user.
        /// - If the user is not found, it returns an HTTP 404 Not Found response.
        /// - If the JSON Patch document is invalid or the update fails, it returns an HTTP 400 Bad Request response.
        /// </returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateUser(int id, [FromBody] JsonPatchDocument<UpdateUserDTO> patchDocument)
        {
            // Check if the user exists
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            // Apply the patch document to the user
            var updatedUser = await _userService.PartiallyUpdateUserAsync(id, patchDocument);

            if (updatedUser != null)
            {
                return Ok(updatedUser);
            }

            return BadRequest("Invalid patch document or user update failed.");
        }


        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok("User deleted successfully.");
        }

        /// <summary>
        /// Retrieves a list of users.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsers(int pageNumber = 1, int pageSize = 10)
        {
            var users = await _userService.GetUsersAsync(pageNumber, pageSize);
            return Ok(users);
        }

        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        [HttpGet("username/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound("User not found.");
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound("User not found.");
        }

        /// <summary>
        /// Searches users based on a search term.
        /// </summary>
        [HttpGet("search/{searchTerm}")]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            var users = await _userService.SearchUsersAsync(searchTerm);
            return Ok(users);
        }

        /// <summary>
        /// Resends the verification email to the user.
        /// </summary>
        /// <param name="email">The email address of the user to whom the verification email should be resent.</param>
        /// <returns>
        /// A success response if the verification email is sent successfully, otherwise returns a bad request response.
        /// </returns>
        [HttpPost("resend-verification-email")]
        public async Task<IActionResult> ResendVerificationEmail(string email)
        {
            var result = await _userService.ResendVerificationEmail(email);
            if (result)
            {
                return Ok("Verification email sent successfully.");
            }
            return BadRequest("Error sending verification email. Please try again.");
        }

    }
}