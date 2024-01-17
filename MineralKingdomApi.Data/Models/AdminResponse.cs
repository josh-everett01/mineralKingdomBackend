using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MineralKingdomApi.Models
{
    public class AdminResponse
    {
        [Key]
        public int ResponseId { get; set; }

        [ForeignKey("CustomerInquiry")]
        public int InquiryId { get; set; }

        // Assuming AdminId is the Id from the User model
        [ForeignKey("User")]
        public int AdminId { get; set; }

        [Required]
        public string ResponseMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual CustomerInquiry CustomerInquiry { get; set; }
        public virtual User Admin { get; set; }
    }
}
