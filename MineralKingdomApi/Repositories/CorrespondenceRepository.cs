using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.Data.Models;
using MineralKingdomApi.Models;

namespace MineralKingdomApi.Repositories
{
    public class CorrespondenceRepository : ICorrespondenceRepository
    {
        private readonly MineralKingdomContext _context;

        public CorrespondenceRepository(MineralKingdomContext context)
        {
            _context = context;
        }

        public async Task AddInquiry(CustomerInquiry inquiry)
        {
            _context.CustomerInquiry.Add(inquiry);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CustomerInquiry>> GetInquiriesByCustomer(int customerId)
        {
            return await _context.CustomerInquiry
                                 .Where(i => i.CustomerId == customerId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<CustomerInquiry>> GetAllInquiries()
        {
            return await _context.CustomerInquiry.ToListAsync();
        }

        public async Task<CustomerInquiry> GetInquiryById(int inquiryId)
        {
            return await _context.CustomerInquiry
                                 .FirstOrDefaultAsync(i => i.InquiryId == inquiryId);
        }

        public async Task AddResponse(AdminResponse response)
        {
            _context.AdminResponse.Add(response);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInquiryStatus(int inquiryId, string status)
        {
            var inquiry = await _context.CustomerInquiry
                                        .FirstOrDefaultAsync(i => i.InquiryId == inquiryId);
            if (inquiry != null)
            {
                inquiry.Status = status;
                inquiry.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
