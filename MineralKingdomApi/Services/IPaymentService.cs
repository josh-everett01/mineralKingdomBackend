using System;
using MineralKingdomApi.DTOs.PaymentDTOs;
using MineralKingdomApi.Models;

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
        Task<IEnumerable<PaymentDetailsDto>> GetPaymentDetailsBySessionIdCollectionAsync(string sessionId);
        Task<bool> CancelPaymentByMineralId(int mineralId);
        Task<bool> CancelPaymentByOrderId(string orderId);
        Task<IEnumerable<PaymentDetailsDto>> GetPaymentDetailsByOrderIdAsync(string orderId);
        Task<IEnumerable<PaymentDetailsDto>> GetPaymentDetailsByMineralIdAsync(int mineralId);
        Task<bool> CancelPayment(string orderId);
        Task SendInvoiceEmail(User user, string sessionId, decimal totalAmount, PaymentDetailsDto pmntDetailsDto);

    }

}

