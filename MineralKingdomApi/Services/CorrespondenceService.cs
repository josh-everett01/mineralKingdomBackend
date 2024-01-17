using System;
using System.Net;
using System.Net.Mail;
using MineralKingdomApi.DTOs.CorrespondenceDTOs;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;

namespace MineralKingdomApi.Services
{
    public class CorrespondenceService : ICorrespondenceService
    {
        private readonly ICorrespondenceRepository _repository;

        public CorrespondenceService(ICorrespondenceRepository repository)
        {
            _repository = repository;
        }

        public async Task SubmitInquiry(InquirySubmissionDto inquiryDto)
        {
            var inquiry = new CustomerInquiry
            {
                CustomerId = inquiryDto.CustomerId,
                Name = inquiryDto.Name,
                Email = inquiryDto.Email,
                Message = inquiryDto.Message,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddInquiry(inquiry);
        }

        public async Task<IEnumerable<CustomerInquiry>> GetInquiriesByCustomer(int customerId)
        {
            return await _repository.GetInquiriesByCustomer(customerId);
        }

        public async Task<IEnumerable<CustomerInquiry>> GetAllInquiries()
        {
            return await _repository.GetAllInquiries();
        }

        public async Task RespondToInquiry(int inquiryId, InquiryResponseDto responseDto)
        {
            var response = new AdminResponse
            {
                InquiryId = inquiryId,
                AdminId = responseDto.AdminId, // Assuming AdminId is determined at this point
                ResponseMessage = responseDto.ResponseMessage,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddResponse(response);
            await _repository.UpdateInquiryStatus(inquiryId, "Responded");
            // Retrieve the inquiry to get the user's email
            var inquiry = await _repository.GetInquiryById(inquiryId);
            if (inquiry != null)
            {
                // Send email notification to the user
                await SendResponseEmail(inquiry.Email, response.ResponseMessage);
            }
        }

        private async Task SendResponseEmail(string userEmail, string adminResponse)
        {
            var fromAddress = new MailAddress("admin@mineralkingdom.com", "Mineral Kingdom Admin");
            var toAddress = new MailAddress(userEmail);
            const string subject = "Response to Your Inquiry";
            string htmlEmailBody = "<html><body style='font-family: Arial, sans-serif; color: #333;'>";
            htmlEmailBody += "<header><h1 style='color: #007bff;'>Mineral Kingdom</h1></header>"; // Header with blue color
            htmlEmailBody += "<p>Dear Customer,</p>";
            htmlEmailBody += $"<p style='color: #555;'>An admin has responded to your inquiry:</p>"; // Gray color
            htmlEmailBody += $"<blockquote style='border-left: 3px solid #007bff; margin-left: 0; padding-left: 10px;'>{adminResponse}</blockquote>"; // Blockquote for response
            htmlEmailBody += "<footer><p style='color: #007bff;'>Best regards,<br>Mineral Kingdom Team</p></footer>"; // Footer with blue color
            htmlEmailBody += "</body></html>";

            var mailtrapUsername = Environment.GetEnvironmentVariable("MAILTRAP_USERNAME");
            var mailtrapPassword = Environment.GetEnvironmentVariable("MAILTRAP_PASSWORD");

            // SMTP client setup (or use an email service provider)
            var smtp = new SmtpClient
            {
                Host = "smtp.mailtrap.io", // SMTP Host from MailTrap
                Port = 587, // SMTP Port from MailTrap
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(mailtrapUsername, mailtrapPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = htmlEmailBody,
                IsBodyHtml = true
            })
            {
                await smtp.SendMailAsync(message);
            }
        }

        public async Task UpdateInquiryStatus(int inquiryId, InquiryStatusUpdateDto statusUpdateDto)
        {
            await _repository.UpdateInquiryStatus(inquiryId, statusUpdateDto.Status);
        }

    }

}

