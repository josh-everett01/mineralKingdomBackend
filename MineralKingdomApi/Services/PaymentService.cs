using System;
using Microsoft.EntityFrameworkCore;
using MineralKingdomApi.Data;
using MineralKingdomApi.Data.Models;
using MineralKingdomApi.DTOs.PaymentDTOs;
using MineralKingdomApi.Repositories;

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
            await _paymentDetailsRepository.UpdatePaymentDetailsAsync(paymentDetailsDto.CheckoutSessionId, paymentDetailsDto.TransactionId, paymentDetailsDto.Status);
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


    }

}

