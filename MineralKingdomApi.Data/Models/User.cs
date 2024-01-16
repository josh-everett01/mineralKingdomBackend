using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.Models
{

    public enum UserRole
    {
        Undefined,
        Admin,
        Customer
    }


    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string? LastName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Username { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(6)]
        public string? Password { get; set; }

        [Required]
        public DateTime RegisteredAt { get; set; }

        // Address Properties
        [Required]
        [MaxLength(100)]
        public string? StreetAddress { get; set; }

        [Required]
        [MaxLength(50)]
        public string? City { get; set; }

        [Required]
        [MaxLength(50)]
        public string? State { get; set; }

        [Required]
        [MaxLength(10)]
        [RegularExpression(@"^\d{5}(-\d{4})?$")]
        public string? ZipCode { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Country { get; set; }

        [Required]
        public UserRole UserRole { get; set; }

        // Email Verification Properties
        public bool EmailVerified { get; set; } = false;
        public string? VerificationToken { get; set; }
        public DateTime? TokenExpirationDate { get; set; }

        // Stripe Customer ID
        public string? StripeCustomerId { get; set; }

        // Refresh token for Login / JWTService
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public bool IsAddressVerified { get; set; } = false;
    }

}

