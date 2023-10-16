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

namespace MineralKingdomApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public UserService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<UserResponseDTO> LoginUserAsync(LoginDTO loginDTO)
        {
            var user = await _userRepository.GetUserByUsernameAsync(loginDTO.Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                // Password is correct, generate a JWT token
                var jwtToken = _jwtService.GenerateJwtToken(user);

                var response = MapToUserResponseDTO(user);
                response.JwtToken = jwtToken;
                return response;
            }

            return null; // User not found or password is incorrect
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
                UserRole = UserRole.Customer,
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
            existingUser.Password = HashPassword(updateUserDTO.Password); // Hash the password
            existingUser.StreetAddress = updateUserDTO.StreetAddress;
            existingUser.City = updateUserDTO.City;
            existingUser.State = updateUserDTO.State;
            existingUser.ZipCode = updateUserDTO.ZipCode;
            existingUser.Country = updateUserDTO.Country;

            await _userRepository.UpdateUserAsync(existingUser);

            return MapToUserResponseDTO(existingUser);
        }

        public async Task<UserResponseDTO> PartiallyUpdateUserAsync(int userId, JsonPatchDocument<UpdateUserDTO> patchDocument)
        {
            // Get the user by ID
            var existingUser = await _userRepository.GetUserByIdAsync(userId);
            if (existingUser == null)
            {
                return null; // User not found
            }

            // Log the patch document to see if it contains the correct value
            foreach (var operation in patchDocument.Operations)
            {
                Console.WriteLine($"Operation: {operation.op}, Path: {operation.path}, Value: {operation.value}");
            }

            // Map the existing user to an UpdateUserDTO
            var userToUpdateDTO = MapToUpdateUserDTO(existingUser);

            // Apply the patch document to the DTO
            patchDocument.ApplyTo(userToUpdateDTO);

            // Update the user with the changes from the patched DTO
            MapUpdateUserDTOToUser(userToUpdateDTO, existingUser);
            await _userRepository.UpdateUserAsync(existingUser);

            return MapToUserResponseDTO(existingUser);
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


