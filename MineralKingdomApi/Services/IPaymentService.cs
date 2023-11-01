using System;
using MineralKingdomApi.DTOs.PaymentDTOs;

namespace MineralKingdomApi.Services
{
    public interface IPaymentService
    {
        Task<PaymentDetailsDto> AddPaymentDetailsAsync(PaymentDetailsDto paymentDetailsDto);
        Task<PaymentDetailsDto> GetPaymentDetailsByTransactionIdAsync(string transactionId);
        Task UpdatePaymentDetailsAsync(PaymentDetailsDto paymentDetailsDto);
        Task UpdateTransactionIdAsync(string checkoutSessionId, string transactionId);
        Task<PaymentDetailsDto> GetPaymentDetailsBySessionIdAsync(string sessionId);
        Task UpdatePaymentDetailsStatusAsync(string transactionId, string status);
    }

}

