﻿using System;
using Microsoft.AspNetCore.Mvc;
using MineralKingdomApi.DTOs.PaymentDTOs;
using MineralKingdomApi.Repositories;
using MineralKingdomApi.Services;
using Stripe.Checkout;

namespace MineralKingdomApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly IMineralRepository _mineralRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentService _paymentService;

        public CheckoutController(IMineralRepository mineralRepository, IUserRepository userRepository, IPaymentService paymentService)
        {
            _mineralRepository = mineralRepository;
            _userRepository = userRepository;
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutRequestDto request)
        {
            // Retrieve user information
            var user = await _userRepository.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Generate a unique order ID
            var orderId = GenerateOrderId(user.Id);

            // Create a list to hold PaymentDetailsDto instances
            var paymentDetailsList = new List<PaymentDetailsDto>();

            // Store initial payment details with orderId for each line item
            foreach (var item in request.LineItems)
            {
                var paymentDetailsDto = new PaymentDetailsDto
                {
                    // Assign values from item and user
                    Amount = item.Price,
                    Currency = "USD",
                    PaymentMethod = "card",
                    Status = "pending",
                    Description = $"Purchase of {item.Name}",
                    CustomerId = user.Id.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    TransactionId = orderId,
                    OrderId = orderId,
                    MineralId = item.MineralId
                };

                paymentDetailsList.Add(paymentDetailsDto);
            }

            var successUrl = $"https://localhost:8080/payment-success/{orderId}";
            var cancelUrl = $"https://localhost:8080/payment-cancelled/{orderId}";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = request.LineItems.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // Convert to cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Name,
                        },
                    },
                    Quantity = item.Quantity,
                }).ToList(),
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            // Update each PaymentDetailsDto with the CheckoutSessionId
            foreach (var paymentDetails in paymentDetailsList)
            {
                paymentDetails.CheckoutSessionId = session.Id;
                await _paymentService.AddPaymentDetailsAsync(paymentDetails);
            }

            // Return session details along with the order ID
            return Ok(new { sessionId = session.Id, url = session.Url, orderId = orderId });
        }


        [HttpPost("purchase-mineral")]
        public async Task<IActionResult> PurchaseMineral([FromBody] MineralPurchaseDto request)
        {
            // 1. Validate the request
            if (request == null || request.MineralId == 0 || request.UserId == 0)
            {
                return BadRequest("Invalid request. Ensure that the mineral ID and user ID are provided.");
            }

            // 2. Retrieve the mineral and user from the database
            var mineral = await _mineralRepository.GetMineralByIdAsync(request.MineralId);
            var user = await _userRepository.GetUserByIdAsync(request.UserId);

            // Check if the mineral and user were successfully retrieved
            if (mineral == null || user == null)
            {
                return NotFound("Mineral or User not found. Ensure that the provided IDs are correct.");
            }

            // 3. Create a Checkout Session for the payment
            var successUrl = $"https://localhost:8080/payment-success/{mineral.Id}";
            var cancelUrl = $"https://localhost:8080/payment-cancelled/{mineral.Id}";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(mineral.Price * 100), // Convert the price to cents
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = mineral.Name,
                    },
                },
                Quantity = 1,
            }
        },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            // 4. Create and store the PaymentDetailsDto
            var paymentDetailsDto = new PaymentDetailsDto
            {
                TransactionId = "",
                Amount = mineral.Price,
                Currency = "USD",
                PaymentMethod = "card",
                Status = "pending",
                Description = $"Purchase of {mineral.Name}",
                CustomerId = user.Id.ToString(),
                CreatedAt = DateTime.UtcNow,
                CheckoutSessionId = session.Id,
                MineralId = mineral.Id,
                OrderId = session.Id
                
            };
         
            await _paymentService.AddPaymentDetailsAsync(paymentDetailsDto);

            // 5. Return the Checkout Session ID and URL to the client
            return Ok(new { sessionId = session.Id, url = session.Url });
        }

        private string GenerateOrderId(int userId)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var randomComponent = new Random().Next(1000, 9999); // Random number between 1000 and 9999
            return $"{userId}-{timestamp}-{randomComponent}";
        }

        [HttpPost("cancel-order")]
        public async Task<IActionResult> CancelOrder([FromBody] string orderId)
        {
            var paymentDetailsList = await _paymentService.GetPaymentDetailsByOrderIdAsync(orderId);
            if (paymentDetailsList == null || !paymentDetailsList.Any())
            {
                return NotFound($"No payment details found for Order ID: {orderId}");
            }

            var result = await _paymentService.CancelPayment(orderId);
            if (!result)
            {
                return BadRequest("Failed to cancel the order.");
            }

            return Ok("Order cancelled successfully.");
        }

        [HttpPost("cancel-mineral-purchase")]
        public async Task<IActionResult> CancelMineralPurchase([FromBody] int mineralId)
        {
            var paymentDetailsList = await _paymentService.GetPaymentDetailsByMineralIdAsync(mineralId);
            if (paymentDetailsList == null || !paymentDetailsList.Any())
            {
                return NotFound($"No payment details found for Mineral ID: {mineralId}");
            }

            // Assuming you have a method in your service to handle cancellation by mineral ID
            var result = await _paymentService.CancelPaymentByMineralId(mineralId);
            if (!result)
            {
                return BadRequest("Failed to cancel the mineral purchase.");
            }

            return Ok("Mineral purchase cancelled successfully.");
        }



    }
}

