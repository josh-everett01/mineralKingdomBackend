using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MineralKingdomApi.DTOs.CorrespondenceDTOs;
using MineralKingdomApi.Services;

namespace MineralKingdomApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CorrespondenceController : ControllerBase
    {
        private readonly ICorrespondenceService _service;

        public CorrespondenceController(ICorrespondenceService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitInquiry([FromBody] InquirySubmissionDto inquiryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.SubmitInquiry(inquiryDto);
            return Ok("Inquiry submitted successfully.");
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetInquiriesByCustomer(int customerId)
        {
            var inquiries = await _service.GetInquiriesByCustomer(customerId);
            return Ok(inquiries);
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAllInquiries()
        {
            var inquiries = await _service.GetAllInquiries();
            return Ok(inquiries);
        }

        [HttpPost("respond/{inquiryId}")]
        public async Task<IActionResult> RespondToInquiry(int inquiryId, [FromBody] InquiryResponseDto responseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.RespondToInquiry(inquiryId, responseDto);
            return Ok("Response submitted successfully.");
        }

        [HttpPut("{inquiryId}/status")]
        public async Task<IActionResult> UpdateInquiryStatus(int inquiryId, [FromBody] InquiryStatusUpdateDto statusUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.UpdateInquiryStatus(inquiryId, statusUpdateDto);
            return Ok("Inquiry status updated successfully.");
        }
    }
}
