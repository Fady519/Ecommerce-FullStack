using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.BasketDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Presentation.Controllers
{
    public class PaymentController : ApiBaseController
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<BasketDTO>> CreateOrUpdatePaymentIntent(string BasketId)
        {
            var Basket = await paymentService.CreateOrUpdatePaymentIntentAsync(BasketId, RequiredUserId);
            return Ok(Basket);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new System.IO.StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"];

            if (string.IsNullOrEmpty(signature))
                return BadRequest();

            var success = await paymentService.ProcessWebhookAsync(json, signature!);
            if (!success)
                return BadRequest();

            return Ok();
        }
    }
}
