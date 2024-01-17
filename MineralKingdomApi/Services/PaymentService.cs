using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.Data.Models;
using MineralKingdomApi.DTOs.PaymentDTOs;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;
using Stripe.Checkout;
using System.Reflection.Metadata;
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Kernel.Colors;
using iText.Layout.Properties;


namespace MineralKingdomApi.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentDetailsRepository _paymentDetailsRepository;
        private readonly ILogger<PaymentService> _logger;
        private readonly MineralKingdomContext _mineralKingdomContext;

        public PaymentService(IPaymentDetailsRepository paymentDetailsRepository, ILogger<PaymentService> logger, MineralKingdomContext mineralKingdomContext)
        {
            _paymentDetailsRepository = paymentDetailsRepository;
            _logger = logger;
            _mineralKingdomContext = mineralKingdomContext;
        }

        public async Task<PaymentDetailsDto> AddPaymentDetailsAsync(PaymentDetailsDto paymentDetailsDto)
        {
            _logger.LogInformation($"Creating PaymentDetails entry with CheckoutSessionId: {paymentDetailsDto.CheckoutSessionId}");
            var paymentDetails = MapToPaymentDetails(paymentDetailsDto);
            _logger.LogInformation($"Payment Details leaving AddPaymentDetailsAsync: {paymentDetails.Id}");
            await _paymentDetailsRepository.AddPaymentDetailsAsync(paymentDetails);
            return MapToPaymentDetailsDto(paymentDetails);
        }

        public async Task<PaymentDetailsDto> GetPaymentDetailsByTransactionIdAsync(string transactionId)
        {
            var paymentDetails = await _paymentDetailsRepository.GetPaymentDetailsByTransactionIdAsync(transactionId);

            if (paymentDetails == null)
            {
                _logger.LogWarning("PaymentDetails not found for TransactionId: {TransactionId}", transactionId);
                return null; // Or return a default value if that makes more sense for your application
            }

            return MapToPaymentDetailsDto(paymentDetails);
        }

        public async Task UpdatePaymentDetailsAsync(PaymentDetailsDto paymentDetailsDto)
        {
            await _paymentDetailsRepository.UpdatePaymentDetailsAsync(paymentDetailsDto.CheckoutSessionId, paymentDetailsDto.TransactionId, paymentDetailsDto.Status, paymentDetailsDto.OrderId);
        }

        public async Task UpdateTransactionIdAsync(string checkoutSessionId, string transactionId)
        {
            var paymentDetails = await _mineralKingdomContext.PaymentDetails
                .FirstOrDefaultAsync(pd => pd.CheckoutSessionId == checkoutSessionId);

            if (paymentDetails != null)
            {
                paymentDetails.TransactionId = transactionId;
                await _mineralKingdomContext.SaveChangesAsync();
            }
        }

        // Add other methods as needed

        private PaymentDetails MapToPaymentDetails(PaymentDetailsDto dto)
        {
            return new PaymentDetails
            {
                Id = dto.MineralId,
                TransactionId = dto.TransactionId,
                Amount = dto.Amount,
                Currency = dto.Currency,
                PaymentMethod = dto.PaymentMethod,
                Status = dto.Status,
                Description = dto.Description,
                CustomerId = dto.CustomerId,
                CreatedAt = dto.CreatedAt,
                CheckoutSessionId = dto.CheckoutSessionId,
                OrderId = dto.OrderId,
            };
        }

        private PaymentDetailsDto MapToPaymentDetailsDto(PaymentDetails entity)
        {
            return new PaymentDetailsDto
            {
                Id = entity.Id,
                TransactionId = entity.TransactionId,
                Amount = entity.Amount,
                Currency = entity.Currency,
                PaymentMethod = entity.PaymentMethod,
                Status = entity.Status,
                Description = entity.Description,
                CustomerId = entity.CustomerId,
                CreatedAt = entity.CreatedAt,
                CheckoutSessionId = entity.CheckoutSessionId,
                OrderId = entity.OrderId,
            };
        }

        public async Task<PaymentDetailsDto?> GetPaymentDetailsBySessionIdAsync(string sessionId)
        {
            var paymentDetails = await _paymentDetailsRepository.GetPaymentDetailsBySessionIdAsync(sessionId);
            _logger.LogWarning("PaymentDetails:", paymentDetails);
            if (paymentDetails == null)
            {
                _logger.LogWarning("PaymentDetails not found for CheckoutSessionId: {CheckoutSessionId}", sessionId);
                return null; // Return null if payment details are not found
            }

            return MapToPaymentDetailsDto(paymentDetails);
        }


        public async Task UpdatePaymentDetailsStatusAsync(string transactionId, string status)
        {
            await _paymentDetailsRepository.UpdatePaymentDetailsStatusAsync(transactionId, status);
        }

        public async Task<IEnumerable<PaymentDetailsDto>> GetPaymentDetailsBySessionIdCollectionAsync(string sessionId)
        {
            var paymentDetailsList = await _mineralKingdomContext.PaymentDetails
                .Where(pd => pd.CheckoutSessionId == sessionId)
                .ToListAsync();

            if (!paymentDetailsList.Any())
            {
                _logger.LogWarning("No PaymentDetails found for CheckoutSessionId: {CheckoutSessionId}", sessionId);
                return Enumerable.Empty<PaymentDetailsDto>(); // Return an empty collection if no details are found
            }

            return paymentDetailsList.Select(pd => MapToPaymentDetailsDto(pd)).ToList();
        }

        public async Task<bool> CancelPaymentByOrderId(string orderId)
        {
            var paymentDetailsList = await _paymentDetailsRepository.GetPaymentDetailsByOrderIdAsync(orderId);
            if (paymentDetailsList == null || !paymentDetailsList.Any())
            {
                _logger.LogWarning($"No PaymentDetails found for OrderId: {orderId}");
                return false;
            }

            foreach (var paymentDetails in paymentDetailsList)
            {
                _mineralKingdomContext.PaymentDetails.Remove(paymentDetails);
            }

            await _mineralKingdomContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelPaymentByMineralId(int mineralId)
        {
            var paymentDetailsList = await _paymentDetailsRepository.GetPaymentDetailsByMineralIdAsync(mineralId);
            if (paymentDetailsList == null || !paymentDetailsList.Any())
            {
                _logger.LogWarning($"No PaymentDetails found for MineralId: {mineralId}");
                return false;
            }

            foreach (var paymentDetails in paymentDetailsList)
            {
                _mineralKingdomContext.PaymentDetails.Remove(paymentDetails);
            }

            await _mineralKingdomContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PaymentDetailsDto>> GetPaymentDetailsByOrderIdAsync(string orderId)
        {
            var paymentDetailsList = await _mineralKingdomContext.PaymentDetails
                .Where(pd => pd.TransactionId == orderId)
                .ToListAsync();

            if (!paymentDetailsList.Any())
            {
                _logger.LogWarning($"No PaymentDetails found for OrderId: {orderId}");
                return Enumerable.Empty<PaymentDetailsDto>();
            }

            return paymentDetailsList.Select(pd => MapToPaymentDetailsDto(pd)).ToList();
        }

        public async Task<IEnumerable<PaymentDetailsDto>> GetPaymentDetailsByMineralIdAsync(int mineralId)
        {
            var paymentDetailsList = await _mineralKingdomContext.PaymentDetails
                .Where(pd => pd.Id == mineralId)
                .ToListAsync();

            if (!paymentDetailsList.Any())
            {
                _logger.LogWarning($"No PaymentDetails found for MineralId: {mineralId}");
                return Enumerable.Empty<PaymentDetailsDto>();
            }

            return paymentDetailsList.Select(pd => MapToPaymentDetailsDto(pd)).ToList();
        }

        public async Task<bool> CancelPayment(string orderId)
        {
            var paymentDetailsList = await _paymentDetailsRepository.GetPaymentDetailsByOrderIdAsync(orderId);
            if (paymentDetailsList == null || !paymentDetailsList.Any())
            {
                _logger.LogWarning($"No PaymentDetails found for OrderId: {orderId}");
                return false;
            }

            foreach (var paymentDetails in paymentDetailsList)
            {
                _mineralKingdomContext.PaymentDetails.Remove(paymentDetails);
            }

            await _mineralKingdomContext.SaveChangesAsync();
            return true;
        }
        // await _paymentService.SendInvoiceEmail(user, session.Id, paymentDetailsDto.Amount, paymentDetailsDto);
        public async Task SendInvoiceEmail(User user, string sessionId, decimal totalAmount, PaymentDetailsDto pmntDetailsDto)
        {
            var paymentDetailsList = await _paymentDetailsRepository.GetPaymentDetailsBySessionIdAsync(sessionId);
            if (paymentDetailsList == null)
            {
                _logger.LogWarning("No PaymentDetails found for SessionId: {SessionId}", sessionId);
                return;
            }

            var fromAddress = new MailAddress("admin@mineralkingdom.com", "Mineral Kingdom Shop and Auction House");
            var toAddress = new MailAddress(user.Email, user.FirstName + " " + user.LastName);
            const string subject = "Confirmation of your recent purchase";
            string emailBody = $"Dear {user.FirstName},\n\n" +
                               $"Thank you for your purchase. Here is your invoice:\n\n";

            // Create HTML version of the message
            string htmlEmailBody = "<html><body style='font-family: Arial, sans-serif; color: #333;'>";
            htmlEmailBody += "<header><h1 style='color: #007bff;'>Mineral Kingdom Store and Auction House</h1></header>"; // Header with blue color
            htmlEmailBody += $"<p>Dear {user.FirstName},</p>";
            htmlEmailBody += "<p style='color: #555;'>Thank you for your purchase. Here is your order: </p>"; // Gray color
            htmlEmailBody += "<ul>";

            var lineItems = await GetLineItemsFromStripeSession(sessionId);

            // Generate PDF Invoice
            // Generate PDF Invoice
            string tempFilePath = Path.GetTempFileName() + "-MineralKingdomInvoice.pdf";
            CreateInvoicePdf(lineItems, totalAmount, tempFilePath);

            CreateInvoicePdf(lineItems, totalAmount, tempFilePath);

            foreach (var paymentDetail in lineItems)
            {
                var lineItem = paymentDetail;

                //emailBody += $"{lineItem.Description}: {lineItem.Description} - {lineItem.Quantity} = ${lineItem.AmountSubtotal}\n";
                string formattedLineItemAmount = (lineItem.AmountSubtotal / 100m).ToString("N2");
                string formattedOutput = $"{lineItem.Description} - {lineItem.Quantity} = ${formattedLineItemAmount}\n";
                emailBody += formattedOutput;

                htmlEmailBody += $"<li>{lineItem.Description} - {lineItem.Quantity} = ${formattedLineItemAmount}</li><br>";
            }

            // Convert totalAmount from cents to dollars and format
            string formattedTotalAmount = (totalAmount / 100m).ToString("N2");

            emailBody += $"\nTotal Amount: ${formattedTotalAmount}\n\n" +
                         $"Best regards,\n\n" +
                         $"Mineral Kingdom Team";

            htmlEmailBody += "</ul>";
            htmlEmailBody += $"<footer><p style='color: #007bff;'>Total Amount: ${formattedTotalAmount}</p></footer>"; // Footer with blue color
            htmlEmailBody += "<p style='color: #555;'>Please see your itemized invoice attached.</p><p style='color: #007bff;'>Best regards,<br>Mineral Kingdom Team</p>"; // Gray and blue colors
            htmlEmailBody += "</body></html>";

            var mailtrapUsername = Environment.GetEnvironmentVariable("MAILTRAP_USERNAME");
            var mailtrapPassword = Environment.GetEnvironmentVariable("MAILTRAP_PASSWORD");

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
                Attachment pdfAttachment = new Attachment(tempFilePath);
                message.Attachments.Add(pdfAttachment);

                await smtp.SendMailAsync(message);
            }

            // Optionally, delete the temp file if you no longer need it
            File.Delete(tempFilePath);
        }

        public async Task<List<Stripe.LineItem>> GetLineItemsFromStripeSession(string sessionId)
        {
            var service = new Stripe.Checkout.SessionService();
            var session = await service.GetAsync(sessionId, new SessionGetOptions
            {
                Expand = new List<string> { "line_items" }
            });

            var lineItems = session.LineItems.Data.Select(li => new Stripe.LineItem
            {
                Description = li.Description,
                Quantity = li.Quantity ?? 0,
                AmountSubtotal = li.AmountSubtotal
            }).ToList();

            return lineItems;
        }

        public void CreateInvoicePdf(List<Stripe.LineItem> lineItems, decimal totalAmount, string filePath)
        {
            PdfWriter writer = new PdfWriter(filePath);
            PdfDocument pdf = new PdfDocument(writer);
            iText.Layout.Document document = new iText.Layout.Document(pdf);

            string headerContent = "Mineral Kingdom Store and Auction House\n\n";
            Paragraph headerParagraph = new Paragraph(headerContent).SetFontColor(ColorConstants.BLUE);
            document.Add(headerParagraph);

            // Table to display line items
            Table table = new Table(new float[] { 3, 2, 2 }); // Adjust column widths as needed
            table.AddHeaderCell("Description");
            table.AddHeaderCell("Quantity");
            table.AddHeaderCell("Amount");

            foreach (var item in lineItems)
            {
                string formattedLineItemAmount = (item.AmountSubtotal / 100m).ToString("N2");
                table.AddCell(item.Description);
                table.AddCell(item.Quantity.ToString());
                table.AddCell("$" + formattedLineItemAmount);
            }

            // Total Amount
            string formattedTotalAmount = (totalAmount / 100m).ToString("N2");
            document.Add(new Paragraph("\nYour Mineral Kingdom Invoice").SetTextAlignment(TextAlignment.CENTER));
            document.Add(table);
            document.Add(new Paragraph("\nTotal Amount: $" + formattedTotalAmount)
                .SetFontColor(ColorConstants.BLUE)
                .SetTextAlignment(TextAlignment.RIGHT));

            document.Close();
        }

        public async Task<IEnumerable<PaymentDetailsDto>> GetAllPaymentDetailsByUser(int userId)
        {
            try
            {
                // Check if the user exists
                var userExists = await _mineralKingdomContext.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    _logger.LogWarning($"User not found for UserId: {userId}");
                    return Enumerable.Empty<PaymentDetailsDto>(); // Or throw an exception if that's more appropriate
                }

                // Query the PaymentDetails table for entries matching the given userId
                var paymentDetailsList = await _mineralKingdomContext.PaymentDetails
                    .Where(pd => pd.CustomerId == userId.ToString())
                    .ToListAsync();

                if (!paymentDetailsList.Any())
                {
                    _logger.LogWarning($"No PaymentDetails found for UserId: {userId}");
                    return Enumerable.Empty<PaymentDetailsDto>();
                }

                // Map the PaymentDetails entities to PaymentDetailsDto objects
                var paymentDetailsDtos = paymentDetailsList.Select(pd => MapToPaymentDetailsDto(pd)).ToList();

                return paymentDetailsDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving payment details for UserId: {userId}. Error: {ex.Message}");
                throw; // Re-throw the exception to handle it further up the call stack or log it as needed
            }
        }



    }

}

