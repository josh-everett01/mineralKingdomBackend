using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.DTOs.UserDTOs
{
    public class PartialUpdateUserDTO
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(100)]
        public string? StreetAddress { get; set; }

        [MaxLength(50)]
        public string? City { get; set; }

        [MaxLength(50)]
        public string? State { get; set; }

        [MaxLength(10)]
        [RegularExpression(@"^\d{5}(-\d{4})?$")]
        public string? ZipCode { get; set; }

        [MaxLength(50)]
        public string? Country { get; set; }

        [MaxLength(50)]
        public string? Username { get; set; }

        // Note: excluding the Password property from the PartialUpdateUserDTO 
        // to ensure that passwords aren't updated using the PATCH method.
    }

}

