using System;
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
                SuccessUrl = "https://example.com/success",
                CancelUrl = "https://example.com/cancel",
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Ok(new
            {
                sessionId = session.Id,
                Url = session.Url
            });
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
                CancelUrl = "https://example.com/cancel",
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
                MineralId = mineral.Id
            };
         
            await _paymentService.AddPaymentDetailsAsync(paymentDetailsDto);

            // 5. Return the Checkout Session ID and URL to the client
            return Ok(new { sessionId = session.Id, url = session.Url });
        }
    }
}

