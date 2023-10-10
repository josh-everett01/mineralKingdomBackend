using System;
namespace MineralKingdomApi.DTOs
{
    public class MineralResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Origin { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageURL { get; set; }
    }

}

