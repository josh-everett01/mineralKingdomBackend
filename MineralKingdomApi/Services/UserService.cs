using System;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using MineralKingdomApi.DTOs.UserDTOs;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;
using BCrypt.Net;
using MineralKingdomApi.DTOs.UserDTOs.MineralKingdomApi.DTOs.UserDTOs;
using System.Net;
using System.Net.Mail;
using MineralKingdomApi.Services;

namespace MineralKingdomApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IShoppingCartService _shoppingCartService;

        public UserService(IUserRepository userRepository, IJwtService jwtService, IShoppingCartService shoppingCartService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<(UserResponseDTO, string, string)> LoginUserAsync(LoginDTO loginDTO)
        {
            var user = await _userRepository.GetUserByUsernameAsync(loginDTO.Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                // Password is correct, generate a JWT token
                var jwtToken = _jwtService.GenerateJwtToken(user);

                // Generate a refresh token
                var refreshToken = _jwtService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Set the expiry time for the refresh token

                // Save the user with the refresh token
                await _userRepository.UpdateUserAsync(user);

                var response = MapToUserResponseDTO(user);
                response.JwtToken = jwtToken;

                var existingCart = await _shoppingCartService.GetCartByUserIdAsync(user.Id);
                if (existingCart == null) { await _shoppingCartService.CreateCartForUserAsync(user.Id); }
                return (response, jwtToken, refreshToken);
            }

            return (null, null, null); // User not found or password is incorrect
        }

        public async Task<UserResponseDTO> RegisterUserAsync(RegisterDTO registerDTO)
        {
            // Check if a user with the same username or email already exists
            if (await _userRepository.CheckUsernameExistsAsync(registerDTO.Username))
            {
                throw new ArgumentException("Username is already taken.");
            }

            if (await _userRepository.CheckEmailExistsAsync(registerDTO.Email))
            {
                throw new ArgumentException("Email is already registered.");
            }

            // Hash the password
            string hashedPassword = HashPassword(registerDTO.Password);

            // Create a new user
            var user = new User
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                Username = registerDTO.Username,
                Password = hashedPassword,
                RegisteredAt = DateTime.UtcNow,
                UserRole = registerDTO.UserRole == UserRole.Admin ? UserRole.Admin : UserRole.Customer,
                StreetAddress = registerDTO.StreetAddress,
                City = registerDTO.City,
                State = registerDTO.State,
                ZipCode = registerDTO.ZipCode,
                Country = registerDTO.Country,
                VerificationToken = GenerateVerificationToken(),
                TokenExpirationDate = DateTime.UtcNow.AddHours(24) // Token valid for 24 hours
            };

            // Save the user to the database
            await _userRepository.CreateUserAsync(user);

            // Send verification email
            await SendVerificationEmail(user);

            return MapToUserResponseDTO(user);
        }

        public async Task<UserResponseDTO> CreateAdminUserAsync(RegisterDTO registerDTO)
        {
            // You might want to add additional checks or logic specific to admin creation here
            registerDTO.UserRole = UserRole.Admin; // Set the user role to Admin

            // You can add additional validation here if needed
            // For example, check if a user with the same username or email already exists

            return await RegisterUserAsync(registerDTO);
        }

        public async Task<UserResponseDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                return null; // Handle not found
            return MapToUserResponseDTO(user);
        }

        public async Task<UserResponseDTO> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null)
                return null; // Handle not found
            return MapToUserResponseDTO(user);
        }

        public async Task<UserResponseDTO> UpdateUserAsync(int userId, UpdateUserDTO updateUserDTO)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(userId);
            if (existingUser == null)
                return null; // Handle not found

            // Update user properties based on the DTO
            existingUser.FirstName = updateUserDTO.FirstName;
            existingUser.LastName = updateUserDTO.LastName;
            existingUser.Email = updateUserDTO.Email;
            existingUser.Username = updateUserDTO.Username;

            if (!string.IsNullOrWhiteSpace(updateUserDTO.Password))
            {
                if (updateUserDTO.Password.Length < 6)
                {
                    // Handle the validation error. You can throw a custom exception or return a specific error response.
                    throw new ArgumentException("Password must be at least 6 characters long.");
                }
                existingUser.Password = HashPassword(updateUserDTO.Password); // Hash the password
            }

            existingUser.StreetAddress = updateUserDTO.StreetAddress;
            existingUser.City = updateUserDTO.City;
            existingUser.State = updateUserDTO.State;
            existingUser.ZipCode = updateUserDTO.ZipCode;
            existingUser.Country = updateUserDTO.Country;

            await _userRepository.UpdateUserAsync(existingUser);

            return MapToUserResponseDTO(existingUser);
        }

        public async Task<UserResponseDTO> PartiallyUpdateUserAsync(int userId, PartialUpdateUserDTO partialUpdateUserDTO)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(userId);
            if (existingUser == null)
                return null; // Handle not found

            if (!string.IsNullOrWhiteSpace(partialUpdateUserDTO.Email))
            {
                existingUser.Email = partialUpdateUserDTO.Email;
            }

            if (!string.IsNullOrWhiteSpace(partialUpdateUserDTO.StreetAddress))
            {
                existingUser.StreetAddress = partialUpdateUserDTO.StreetAddress;
            }

            if (!string.IsNullOrWhiteSpace(partialUpdateUserDTO.City))
            {
                existingUser.City = partialUpdateUserDTO.City;
            }

            if (!string.IsNullOrWhiteSpace(partialUpdateUserDTO.State))
            {
                existingUser.State = partialUpdateUserDTO.State;
            }

            if (!string.IsNullOrWhiteSpace(partialUpdateUserDTO.ZipCode))
            {
                existingUser.ZipCode = partialUpdateUserDTO.ZipCode;
            }

            if (!string.IsNullOrWhiteSpace(partialUpdateUserDTO.Country))
            {
                existingUser.Country = partialUpdateUserDTO.Country;
            }

            if (!string.IsNullOrWhiteSpace(partialUpdateUserDTO.LastName))
            {
                existingUser.LastName = partialUpdateUserDTO.LastName;
            }

            if (!string.IsNullOrWhiteSpace(partialUpdateUserDTO.FirstName))
            {
                existingUser.FirstName = partialUpdateUserDTO.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(partialUpdateUserDTO.Username))
            {
                existingUser.Username = partialUpdateUserDTO.Username;
            }

            await _userRepository.UpdateUserAsync(existingUser);

            return MapToUserResponseDTO(existingUser);
        }

        public async Task InvalidateRefreshToken(int userId)
        {
            await _userRepository.InvalidateRefreshToken(userId);
        }

        private UpdateUserDTO MapToUpdateUserDTO(User user)
        {
            var updateUserDTO = new UpdateUserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.Username,
                Password = user.Password, // You may want to skip this or apply your own logic
                StreetAddress = user.StreetAddress,
                City = user.City,
                State = user.State,
                ZipCode = user.ZipCode,
                Country = user.Country
            };

            return updateUserDTO;
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetUsersAsync(int pageNumber, int pageSize)
        {
            var users = await _userRepository.GetUsersAsync(pageNumber, pageSize);
            return MapToUserResponseDTOList(users);
        }

        public async Task<UserResponseDTO> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return null; // Handle not found
            return MapToUserResponseDTO(user);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetUsersByRoleAsync(UserRole role)
        {
            var users = await _userRepository.GetUsersByRoleAsync(role);
            return MapToUserResponseDTOList(users);
        }

        public async Task<bool> CheckUsernameExistsAsync(string username)
        {
            return await _userRepository.CheckUsernameExistsAsync(username);
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userRepository.CheckEmailExistsAsync(email);
        }

        public async Task ChangePasswordAsync(int userId, string newPassword)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(userId);
            if (existingUser == null)
                return; // Handle not found

            existingUser.Password = HashPassword(newPassword); // Hash the new password
            await _userRepository.UpdateUserAsync(existingUser);
        }

        public async Task AssignRoleAsync(int userId, UserRole role)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(userId);
            if (existingUser == null)
                return; // Handle not found

            existingUser.UserRole = role;
            await _userRepository.UpdateUserAsync(existingUser);
        }

        public async Task<IEnumerable<UserResponseDTO>> SearchUsersAsync(string searchTerm)
        {
            var users = await _userRepository.SearchUsersAsync(searchTerm);
            return MapToUserResponseDTOList(users);
        }

        private string HashPassword(string password)
        {
            // Generate a salt (a random string) and hash the password with it
            string salt = BCrypt.Net.BCrypt.GenerateSalt(12); // You can adjust the salt work factor as needed
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hashedPassword;
        }

        public string GenerateVerificationToken()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task SendVerificationEmail(User user)
        {
            string verificationLink = $"https://yourwebsite.com/verify?token={user.VerificationToken}";

            var fromAddress = new MailAddress("josh@testing.com", "MineralKingdom");
            var toAddress = new MailAddress(user.Email, user.FirstName + " " + user.LastName);
            const string subject = "Email Verification";
            string body = $"Please click on the following link to verify your email: {verificationLink}";

            var mailtrapUsername = Environment.GetEnvironmentVariable("MAILTRAP_USERNAME");
            var mailtrapPassword = Environment.GetEnvironmentVariable("MAILTRAP_PASSWORD");

            var smtp = new SmtpClient
            {
                Host = "smtp.mailtrap.io", // SMTP Host from MailTrap
                Port = 587, // SMTP Port from MailTrap
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mailtrapUsername, mailtrapPassword)

            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                await smtp.SendMailAsync(message);
            }
        }

        public enum EmailVerificationResult
        {
            Success,
            TokenExpired,
            TokenInvalid,
            Error
        }

        public async Task<EmailVerificationResult> VerifyUserEmail(string token)
        {
            try
            {
               
                var user = await _userRepository.GetUserByVerificationToken(token);
                if (user == null)
                {
                    return EmailVerificationResult.TokenInvalid; // Invalid token
                }

                if (user.TokenExpirationDate < DateTime.UtcNow)
                {
                    return EmailVerificationResult.TokenExpired; // Token expired
                }

                user.EmailVerified = true;
                user.VerificationToken = "Verified"; // Set it to a non-null value or an empty string
                user.TokenExpirationDate = null; // Clear the expiration date

                await _userRepository.UpdateUserAsync(user);
                var existingCart = await _shoppingCartService.GetCartByUserIdAsync(user.Id);
                if (existingCart == null) { await _shoppingCartService.CreateCartForUserAsync(user.Id); }
                return EmailVerificationResult.Success; // Email verified successfully
            }
            catch (Exception ex)
            {
                // Log the exception (assuming you have a logging mechanism in place)
                // _logger.LogError(ex, "Error verifying user email.");
                return EmailVerificationResult.Error; // Generic error
            }
        }

        public async Task<bool> ResendVerificationEmail(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return false; // User not found
            }

            user.VerificationToken = GenerateVerificationToken();
            user.TokenExpirationDate = DateTime.UtcNow.AddHours(24);
            await _userRepository.UpdateUserAsync(user);

            await SendVerificationEmail(user);
            return true; // Verification email sent successfully
        }

        // Mappers

        // Helper method to map User to UserResponseDTO
        private UserResponseDTO MapToUserResponseDTO(User user)
        {
            return new UserResponseDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Username = user.Username,
                UserRole = user.UserRole,
                RegisteredAt = user.RegisteredAt
                // Map other properties as needed
            };
        }

        // Helper method to map a list of Users to UserResponseDTOs
        private IEnumerable<UserResponseDTO> MapToUserResponseDTOList(IEnumerable<User> users)
        {
            var userResponseDTOs = new List<UserResponseDTO>();
            foreach (var user in users)
            {
                userResponseDTOs.Add(MapToUserResponseDTO(user));
            }
            return userResponseDTOs;
        }

        // Helper method to map properties from UpdateUserDTO to User
        private void MapUpdateUserDTOToUser(UpdateUserDTO updateUserDTO, User user)
        {
            // Map the properties from UpdateUserDTO to User
            user.FirstName = updateUserDTO.FirstName;
            user.LastName = updateUserDTO.LastName;
            user.Email = updateUserDTO.Email;
            user.Username = updateUserDTO.Username;
            user.Password = HashPassword(updateUserDTO.Password); // Hash the password
            user.StreetAddress = updateUserDTO.StreetAddress;
            user.City = updateUserDTO.City;
            user.State = updateUserDTO.State;
            user.ZipCode = updateUserDTO.ZipCode;
            user.Country = updateUserDTO.Country;
        }

    }
}


