using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.DTOs.UserDTOs
{
    namespace MineralKingdomApi.DTOs.UserDTOs
    {
        public class LoginDTO
        {
            [Required]
            [MaxLength(50)]
            public string Username { get; set; }

            [Required]
            [MaxLength(100)]
            [MinLength(6)]
            public string Password { get; set; }
        }
    }

}

