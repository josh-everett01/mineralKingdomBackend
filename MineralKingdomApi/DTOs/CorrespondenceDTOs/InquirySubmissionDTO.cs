using System;
namespace MineralKingdomApi.DTOs.CorrespondenceDTOs
{
    public class InquirySubmissionDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }
}

