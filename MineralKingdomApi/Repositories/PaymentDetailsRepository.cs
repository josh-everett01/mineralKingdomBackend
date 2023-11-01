using System;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.Data.Models;

namespace MineralKingdomApi.Repositories
{
    public class PaymentDetailsRepository : IPaymentDetailsRepository
    {
        private readonly MineralKingdomContext _context;

        public PaymentDetailsRepository(MineralKingdomContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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

        public async Task UpdatePaymentDetailsAsync(string checkoutSessionId, string transactionId)
        {
            var existingEntity = _context.ChangeTracker.Entries<PaymentDetails>()
                .FirstOrDefault(e => e.Entity.CheckoutSessionId == checkoutSessionId)?.Entity;

            if (existingEntity != null)
            {
                existingEntity.TransactionId = transactionId;
            }
            else
            {
                var paymentDetails = new PaymentDetails { CheckoutSessionId = checkoutSessionId, TransactionId = transactionId, Status = existingEntity.Status };
                _context.PaymentDetails.Update(paymentDetails);
            }
            await _context.SaveChangesAsync();
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
    }
}

