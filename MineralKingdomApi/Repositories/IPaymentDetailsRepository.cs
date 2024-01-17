using System;
using MineralKingdomApi.Data.Models;

namespace MineralKingdomApi.Repositories
{
    public interface IPaymentDetailsRepository
    {
        Task AddPaymentDetailsAsync(PaymentDetails paymentDetails);
        Task<PaymentDetails> GetPaymentDetailsByTransactionIdAsync(string transactionId);
        Task UpdatePaymentDetailsAsync(string checkoutSessionId, string transactionId, string status, string orderId);
        Task UpdateTransactionIdAsync(string checkoutSessionId, string transactionId);
        Task<PaymentDetails> GetPaymentDetailsBySessionIdAsync(string sessionId);
        Task UpdatePaymentDetailsStatusAsync(string transactionId, string status);
        Task<IEnumerable<PaymentDetails>> GetPaymentDetailsByOrderIdAsync(string orderId);
        Task<IEnumerable<PaymentDetails>> GetPaymentDetailsByMineralIdAsync(int mineralId);
    }

}

