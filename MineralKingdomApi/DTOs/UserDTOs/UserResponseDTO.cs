﻿using System;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.DTOs.UserDTOs
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public UserRole UserRole { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string JwtToken { get; set; }

        // Add address properties here
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
}