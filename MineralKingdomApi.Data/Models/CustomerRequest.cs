using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MineralKingdomApi.Data.Models
{
    public class CustomerRequest
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public Address Address { get; set; }

    }

    public class Address
    {
        [Required]
        [MaxLength(200)]
        public string Line1 { get; set; }

        [MaxLength(200)]
        public string Line2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        [Required]
        [MaxLength(10)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(50)]
        public string Country { get; set; }
    }

}

