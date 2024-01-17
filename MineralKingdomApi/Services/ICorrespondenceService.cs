using System.Collections.Generic;
using System.Threading.Tasks;
using MineralKingdomApi.DTOs;
using MineralKingdomApi.DTOs.CorrespondenceDTOs;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Services
{
    public interface ICorrespondenceService
    {
        Task SubmitInquiry(InquirySubmissionDto inquiryDto);
        Task<IEnumerable<CustomerInquiry>> GetInquiriesByCustomer(int customerId);
        Task<IEnumerable<CustomerInquiry>> GetAllInquiries();
        Task RespondToInquiry(int inquiryId, InquiryResponseDto responseDto);
        Task UpdateInquiryStatus(int inquiryId, InquiryStatusUpdateDto statusUpdateDto);
    }
}
