using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.Models
{
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
        [MaxLength(100)]
        public string? StreetAddress { get; set; }

        [MaxLength(50)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? State { get; set; }

        [MaxLength(10)]
        [RegularExpression(@"^\d{5}(-\d{4})?$")]
        public string? ZipCode { get; set; }

        // Add more properties as needed, such as user roles, etc.
    }
}
