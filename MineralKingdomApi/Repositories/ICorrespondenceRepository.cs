using System.Collections.Generic;
using System.Threading.Tasks;
using MineralKingdomApi.Data.Models;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Repositories
{
    public interface ICorrespondenceRepository
    {
        Task AddInquiry(CustomerInquiry inquiry);
        Task<IEnumerable<CustomerInquiry>> GetInquiriesByCustomer(int customerId);
        Task<IEnumerable<CustomerInquiry>> GetAllInquiries();
        Task<CustomerInquiry> GetInquiryById(int inquiryId);
        Task AddResponse(AdminResponse response);
        Task UpdateInquiryStatus(int inquiryId, string status);
    }
}
