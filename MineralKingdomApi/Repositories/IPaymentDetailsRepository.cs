using System;
using MineralKingdomApi.Data.Models;

namespace MineralKingdomApi.Repositories
{
    public interface IPaymentDetailsRepository
    {
        Task AddPaymentDetailsAsync(PaymentDetails paymentDetails);
        Task<PaymentDetails> GetPaymentDetailsByTransactionIdAsync(string transactionId);
        Task UpdatePaymentDetailsAsync(string paymentDetailsId, string transactionId);
        Task UpdateTransactionIdAsync(string checkoutSessionId, string transactionId);
        Task<PaymentDetails> GetPaymentDetailsBySessionIdAsync(string sessionId);
        Task UpdatePaymentDetailsStatusAsync(string transactionId, string status);
    }

}

