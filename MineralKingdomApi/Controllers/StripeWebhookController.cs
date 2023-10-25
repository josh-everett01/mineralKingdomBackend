using System;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using MineralKingdomApi.Repositories;
using Stripe;
using Stripe.BillingPortal;

namespace MineralKingdomApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IMineralRepository _mineralRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IMineralRepository mineralRepository, IUserRepository userRepository, ILogger<WebhookController> logger)
        {
            _mineralRepository = mineralRepository ?? throw new ArgumentNullException(nameof(mineralRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> HandleStripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], "your_stripe_endpoint_secret");

            if (stripeEvent.Type == Events.ChargeSucceeded)
            {
                var charge = stripeEvent.Data.Object as Charge;
                if (charge == null)
                {
                    _logger.LogError("Charge object is null");
                    return BadRequest();
                }

                //// Here you can update the mineral status or perform any other necessary actions
                ////var mineralId = /* Extract mineral ID from charge metadata or description */
                //var mineral = await _mineralRepository.GetMineralByIdAsync(mineralId);
                //if (mineral == null)
                //{
                //    _logger.LogError($"Mineral with ID {mineralId} not found");
                //    return NotFound();
                //}

                //mineral.Status = MineralStatus.Sold;
                //await _mineralRepository.UpdateMineral(mineral);

                return Ok();
            }

            return Ok();
        }
    }


}

