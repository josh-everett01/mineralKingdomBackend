using System;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.DTOs
{
    public class UpdateMineralDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Origin { get; set; }
        public string? ImageURL { get; set; }
        public List<string>? ImageURLs { get; set; } = new List<string>();
        public string? VideoURL { get; set; }
        public MineralStatus? Status { get; set; }
        public bool? IsAuctionItem { get; set; }
    }
}
