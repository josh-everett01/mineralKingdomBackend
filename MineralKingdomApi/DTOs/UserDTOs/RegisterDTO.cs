using System;
using System.ComponentModel.DataAnnotations;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.DTOs.UserDTOs
{
	public class RegisterDTO
	{
		[Required]
		[MaxLength(50)]
		public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        public string StreetAddress { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [Required]
        [MaxLength(50)]
        public string State { get; set; }

        [Required]
        [MaxLength(10)]
        [RegularExpression(@"^\d{5}(-\d{4})?$")]
        public string ZipCode { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Country { get; set; }

        public UserRole UserRole { get; set; }

    }
}

