using System;
using MineralKingdomApi.DTOs.UserDTOs;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Repositories
{
    public interface IUserRepository
    {
        // Authenticate and retrieve a user by their username and password
        Task<User> LoginUserAsync(string username, string password);

        // Register a new user based on the provided registration information.
        Task<User> RegisterUserAsync(RegisterDTO registerDTO);

        // Retrieve a user by their ID
        Task<User> GetUserByIdAsync(int id);

        // Retrieve a user by their username
        Task<User> GetUserByUsernameAsync(string username);

        // Create a new user
        Task CreateUserAsync(User user);

        // Update user details
        Task UpdateUserAsync(User user);

        // Delete a user
        Task DeleteUserAsync(int id);

        // Retrieve a list of all users, possibly with pagination
        Task<IEnumerable<User>> GetUsersAsync(int pageNumber, int pageSize);

        // Retrieve a user by their email address
        Task<User> GetUserByEmailAsync(string email);

        // Retrieve users based on their role
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);

        // Check if a username is already taken during registration
        Task<bool> CheckUsernameExistsAsync(string username);

        // Check if an email is already registered
        Task<bool> CheckEmailExistsAsync(string email);

        // Support changing a user's password
        Task ChangePasswordAsync(int userId, string newPassword);

        // Change or assign a role to a user
        Task AssignRoleAsync(int userId, UserRole role);

        // Search users based on certain criteria (e.g., name, email)
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);

        // Retrieve a user based on the verification token.
        Task<User> GetUserByVerificationToken(string token);

        // Invalidate Refresh Token upon logout
        Task InvalidateRefreshToken(int userId);
    }

}

