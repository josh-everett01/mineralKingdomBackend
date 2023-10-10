using System;
namespace MineralKingdomApi.DTOs
{
    public class CreateMineralDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public string? Origin { get; set; }
        public string? ImageURL { get; set; }
    }
}

