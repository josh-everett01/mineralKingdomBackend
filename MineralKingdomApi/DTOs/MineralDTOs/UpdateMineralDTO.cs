using System;
namespace MineralKingdomApi.DTOs
{
    public class UpdateMineralDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Origin { get; set; }
    }
}

