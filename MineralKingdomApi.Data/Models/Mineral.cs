using System;
using System.ComponentModel.DataAnnotations;

namespace MineralKingdomApi.Models
{

    public enum MineralStatus
    {
        Available,
        Sold,
    }

    public class Mineral
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(100)]
        public string? Origin { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [MaxLength(255)]
        public string? ImageURL { get; set; }
        // Add more properties as needed, such as image URLs, properties, etc.

        public List<string>? ImageURLs { get; set; } = new List<string>();

        public string? VideoURL { get; set; }

        [Required]
        public MineralStatus Status { get; set; } = MineralStatus.Available;

    }
}
