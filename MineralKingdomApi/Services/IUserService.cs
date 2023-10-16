using System;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using MineralKingdomApi.DTOs.UserDTOs;
using MineralKingdomApi.DTOs.UserDTOs.MineralKingdomApi.DTOs.UserDTOs;
using MineralKingdomApi.Models;
using static MineralKingdomApi.Services.UserService;

namespace MineralKingdomApi.Services
{
    public interface IUserService
    {
        Task<UserResponseDTO> RegisterUserAsync(RegisterDTO registerDTO);
        Task<UserResponseDTO> LoginUserAsync(LoginDTO loginDTO);
        Task<UserResponseDTO> GetUserByIdAsync(int id);
        Task<UserResponseDTO> GetUserByUsernameAsync(string username);
        Task<UserResponseDTO> UpdateUserAsync(int userId, UpdateUserDTO updateUserDTO);
        Task DeleteUserAsync(int id);
        Task<IEnumerable<UserResponseDTO>> GetUsersAsync(int pageNumber, int pageSize);
        Task<UserResponseDTO> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserResponseDTO>> GetUsersByRoleAsync(UserRole role);
        Task<bool> CheckUsernameExistsAsync(string username);
        Task<bool> CheckEmailExistsAsync(string email);
        Task ChangePasswordAsync(int userId, string newPassword);
        Task AssignRoleAsync(int userId, UserRole role);
        Task<IEnumerable<UserResponseDTO>> SearchUsersAsync(string searchTerm);
        Task<bool> ResendVerificationEmail(string email);
        Task<EmailVerificationResult> VerifyUserEmail(string token);
        Task<UserResponseDTO> PartiallyUpdateUserAsync(int userId, PartialUpdateUserDTO partialUpdateUserDTO);
    }
}


