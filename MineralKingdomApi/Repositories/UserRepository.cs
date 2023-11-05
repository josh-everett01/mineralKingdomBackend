using MineralKingdomApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MineralKingdomApi.Data;
using BCrypt.Net;
using MineralKingdomApi.DTOs.UserDTOs;

namespace MineralKingdomApi.Repositories
{
	public class UserRepository : IUserRepository
	{
        private readonly MineralKingdomContext _context;

        public UserRepository(MineralKingdomContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username)) return null; // Null handling
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task CreateUserAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user)); // Null handling
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<User>> GetUsersAsync(int pageNumber, int pageSize)
        {
            return await _context.Users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return null; // Null handling
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _context.Users.Where(u => u.UserRole == role).ToListAsync();
        }

        public async Task<bool> CheckUsernameExistsAsync(string username)
        {
            if (string.IsNullOrEmpty(username)) return false; // Null handling
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) return false; // Null handling
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await GetUserByIdAsync(userId);
            if (user != null)
            {
                user.Password = newPassword; // Note: Ensure you hash the password before saving!
                await UpdateUserAsync(user);
            }
        }

        public async Task AssignRoleAsync(int userId, UserRole role)
        {
            var user = await GetUserByIdAsync(userId);
            if (user != null)
            {
                user.UserRole = role;
                await UpdateUserAsync(user);
            }
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return new List<User>(); // Null handling
            return await _context.Users
                .Where(u => u.Username.Contains(searchTerm) || u.Email.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<User> LoginUserAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user; // Password is correct, return the user
            }

            return null; // User not found or password is incorrect
        }

        public async Task<User> RegisterUserAsync(RegisterDTO registerDTO)
        {
            // Check if the username or email already exists
            if (await CheckUsernameExistsAsync(registerDTO.Username) || await CheckEmailExistsAsync(registerDTO.Email))
            {
                return null; // Username or email is already taken
            }

            // Hash the password before saving it to the database
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

            var user = new User
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                Username = registerDTO.Username,
                Password = hashedPassword, // Hashed password
                RegisteredAt = DateTime.UtcNow,
                UserRole = UserRole.Customer // Set the user role as needed
                                             
            };

            user.VerificationToken = GenerateVerificationToken();

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetUserByVerificationToken(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
        }

        public string GenerateVerificationToken()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task InvalidateRefreshToken(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _context.SaveChangesAsync();
            }
        }
    }
}

