using System;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.Data.Models;
using MineralKingdomApi.Services;

namespace MineralKingdomApi.Repositories
{
    public class PaymentDetailsRepository : IPaymentDetailsRepository
    {
        private readonly MineralKingdomContext _context;
        private readonly ILogger<PaymentDetailsRepository> _logger;

        public PaymentDetailsRepository(MineralKingdomContext context, ILogger<PaymentDetailsRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

        public async Task AddPaymentDetailsAsync(PaymentDetails paymentDetails)
        {
            await _context.PaymentDetails.AddAsync(paymentDetails);
            await _context.SaveChangesAsync();
        }

        public async Task<PaymentDetails> GetPaymentDetailsByTransactionIdAsync(string transactionId)
        {
            return await _context.PaymentDetails.FirstOrDefaultAsync(pd => pd.TransactionId == transactionId);
        }

        public async Task UpdatePaymentDetailsAsync(string checkoutSessionId, string transactionId, string status)
        {
            var paymentDetailsList = await _context.PaymentDetails
                .Where(pd => pd.CheckoutSessionId == checkoutSessionId)
                .ToListAsync();

            if (paymentDetailsList.Any())
            {
                foreach (var paymentDetails in paymentDetailsList)
                {
                    paymentDetails.TransactionId = transactionId;
                    paymentDetails.Status = status;
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                // Log the case where no matching records are found
                _logger.LogWarning($"No PaymentDetails found for CheckoutSessionId: {checkoutSessionId}");
                // Optionally, you can also handle this scenario differently if needed
            }
        }


        public async Task UpdateTransactionIdAsync(string checkoutSessionId, string transactionId)
        {
            var paymentDetails = await _context.PaymentDetails
                .FirstOrDefaultAsync(pd => pd.CheckoutSessionId == checkoutSessionId);

            if (paymentDetails == null)
            {
                throw new InvalidOperationException($"No PaymentDetails found with CheckoutSessionId: {checkoutSessionId}");
            }

            paymentDetails.TransactionId = transactionId;
            _context.Entry(paymentDetails).State = EntityState.Modified; // Ensure the entity is being tracked and marked as modified
            await _context.SaveChangesAsync();
        }

        public async Task<PaymentDetails> GetPaymentDetailsBySessionIdAsync(string sessionId)
        {
            return await _context.PaymentDetails
                                 .FirstOrDefaultAsync(pd => pd.CheckoutSessionId == sessionId);
        }

        public async Task UpdatePaymentDetailsStatusAsync(string transactionId, string status)
        {
            var paymentDetails = await _context.PaymentDetails
                .FirstOrDefaultAsync(pd => pd.TransactionId == transactionId);

            if (paymentDetails != null)
            {
                paymentDetails.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<PaymentDetails>> GetPaymentDetailsByOrderIdAsync(string orderId)
        {
            return await _context.PaymentDetails
                                .Where(pd => pd.TransactionId == orderId)
                                .ToListAsync();
        }


        public async Task<IEnumerable<PaymentDetails>> GetPaymentDetailsByMineralIdAsync(int mineralId)
        {
            return await _context.PaymentDetails
                                .Where(pd => pd.Id == mineralId)
                                .ToListAsync();
        }

    }
}

