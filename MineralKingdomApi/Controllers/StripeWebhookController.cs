using System;
using System.Net.NetworkInformation;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MineralKingdomApi.DTOs.PaymentDTOs;
using MineralKingdomApi.Models;
using MineralKingdomApi.Repositories;
using MineralKingdomApi.Services;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;

namespace MineralKingdomApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IMineralRepository _mineralRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<WebhookController> _logger;
        private readonly string _stripeEndpointSecret;
        private readonly IPaymentService _paymentService;

        public WebhookController(IMineralRepository mineralRepository, IUserRepository userRepository, ILogger<WebhookController> logger, IConfiguration configuration, IPaymentService paymentService)
        {
            _mineralRepository = mineralRepository ?? throw new ArgumentNullException(nameof(mineralRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stripeEndpointSecret = configuration["STRIPE_WEBHOOK_SECRET"] ?? throw new InvalidOperationException("Stripe endpoint secret is not configured.");
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> HandleStripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            //_logger.LogInformation("This is what is coming in from Stripe: " + json);
            var stripeSignatureHeader = Request.Headers["Stripe-Signature"];
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignatureHeader, _stripeEndpointSecret);
                
                switch (stripeEvent.Type)
                {
                    case Events.CheckoutSessionCompleted:
                        await HandleCheckoutSessionCompleted(stripeEvent);
                        break;
                    case Events.PaymentIntentSucceeded:
                        await HandlePaymentIntentSucceeded(stripeEvent);
                        break;
                    case Events.PaymentIntentCreated:
                        HandlePaymentIntentCreated(stripeEvent);
                        break;
                    case Events.ChargeSucceeded:
                        await HandleChargeSucceeded(stripeEvent);
                        break;
                    default:
                        _logger.LogInformation("Unhandled event type: {EventType}", stripeEvent.Type);
                        break;
                }
            }
            catch (StripeException e)
            {
                _logger.LogError("Stripe webhook failed with error: {ErrorMessage}", e.Message);
                return BadRequest();
            }
            return Ok();
        }

        private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
        {
            var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
            if (session != null)
            {
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.GetAsync(session.PaymentIntentId);

                var paymentDetailsDto = await _paymentService.GetPaymentDetailsBySessionIdAsync(session.Id);
                if (paymentDetailsDto != null)
                {
                    paymentDetailsDto.TransactionId = session.PaymentIntentId;
                    paymentDetailsDto.Status = paymentIntent.Status;
                    await _paymentService.UpdatePaymentDetailsAsync(paymentDetailsDto);

                    if (paymentIntent.Status == "succeeded")
                    {
                        _logger.LogInformation("Mineral status in progresss updating to Sold for MineralId: " + paymentDetailsDto.Id) ;
                        await _mineralRepository.UpdateMineralStatusAsync(paymentDetailsDto.Id, MineralStatus.Sold);
                        _logger.LogInformation("Mineral status updated to Sold for MineralId: " + paymentDetailsDto.Id);
                        await _paymentService.UpdatePaymentDetailsStatusAsync(paymentDetailsDto.TransactionId, paymentIntent.Status);
                    }
                    else
                    {
                        _logger.LogWarning("Payment was not successful. Status: " + paymentIntent.Status);
                    }
                }
                else
                {
                    _logger.LogWarning("PaymentDetails not found for CheckoutSessionId: " + session.Id);
                }
            }
        }




        private async Task HandlePaymentIntentSucceeded(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent != null)
            {
                _logger.LogInformation("PaymentIntent succeeded with ID: " + paymentIntent.Id);
                // Now retrieve the PaymentDetails using the TransactionId
                var paymentDetailsDto = await _paymentService.GetPaymentDetailsByTransactionIdAsync(paymentIntent.Id);
                if (paymentDetailsDto != null)
                {
                    paymentDetailsDto.Status = "succeeded";
                    await _paymentService.UpdatePaymentDetailsAsync(paymentDetailsDto);
                    _logger.LogInformation("PaymentDetails status updated to succeeded for TransactionId: " + paymentIntent.Id);
                }
                else
                {
                    _logger.LogWarning("Payment details not found for transaction ID: " + paymentIntent.Id);
                }
            }
        }

        private void HandlePaymentIntentCreated(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent != null)
            {
                _logger.LogInformation("PaymentIntent created with ID: {PaymentIntentId}", paymentIntent.Id);
                // Add any additional logic if needed
            }
            else
            {
                _logger.LogWarning("Received PaymentIntent event, but could not cast to PaymentIntent.");
            }
        }

        private async Task HandleChargeSucceeded(Event stripeEvent)
        {
            var charge = stripeEvent.Data.Object as Charge;
            if (charge != null)
            {
                _logger.LogInformation("Charge succeeded with ID: {ChargeId}", charge.Id);
                // Update the PaymentDetails status here if needed
                await _paymentService.UpdatePaymentDetailsStatusAsync(charge.PaymentIntentId, "succeeded");
            }
        }
    }
}


