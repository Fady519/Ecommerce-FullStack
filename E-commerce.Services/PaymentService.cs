using E_commerce.Services.Mapping;
using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.BasketModule;
using E_commerce.Domain.Entities.OrderModule;
using E_commerce.Services.Exceptions;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.BasketDTOs;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;
        private readonly IPictureUrlBuilder pictures;

        public PaymentService(IBasketRepository basketRepository , IUnitOfWork unitOfWork , IConfiguration configuration,
            IPictureUrlBuilder pictures)
        {
            this.basketRepository = basketRepository;
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
            this.pictures = pictures;
        }

        public async Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string BasketId, string userId)
        {

            // 1 ) Configure Stripe => Download Package "Stripe.Net"
            // 2 ) Get Basket Id
            
           

            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];

            var Basket = await basketRepository.GetBasketAsync(BasketId);
            // Someone else's basket looks exactly like a missing one (404).
            if(Basket == null || !Basket.CanBeUsedBy(userId))
            {
                throw new BasketNotFoundException(BasketId);
            }

            // 3 ) Amount — always from the catalog price (the basket is loaded with its products), never from the client.

            // A delivery method must be chosen before paying: its price is part of the amount.
            if (Basket.DeliveryMethodId is not int deliveryMethodId)
                throw new DeliveryMethodNotFoundException(null);
            var DeliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>().GetById(deliveryMethodId)
                ?? throw new DeliveryMethodNotFoundException(deliveryMethodId);

            var total = Basket.Items.Sum(item => item.Quantity * item.Product.Price) + DeliveryMethod.Price;

            // Stripe wants the smallest unit (piastres): 120.50 EGP → 12050.
            // The old code cast to long BEFORE multiplying, so 120.50 became 12000 (the .50 was lost).
            var BasketAmount = (long)Math.Round(total * 100, MidpointRounding.AwayFromZero);

            // 4 ) Create PaymentIntent
            var PaymentService = new PaymentIntentService();
            if(Basket.PaymentIntentId == null)
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = BasketAmount,
                    Currency = "egp", // product prices are in Egyptian pounds
                    // Stripe decides which payment methods to offer (cards are always included).
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true }
                };
                var PaymentIntent = await PaymentService.CreateAsync(options);
                Basket.PaymentIntentId = PaymentIntent.Id;
                Basket.ClientSecret = PaymentIntent.ClientSecret;
            }
            // Update
            else
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = BasketAmount,
                };
                await PaymentService.UpdateAsync(Basket.PaymentIntentId, options);

            }

            var saved = await basketRepository.CreateOrUpdateBasketAsync(Basket);
            return saved.ToDto(pictures);

        }

        public async Task<bool> ProcessWebhookAsync(string json, string signature)
        {
            try
            {
                var stripeEvent = Stripe.EventUtility.ConstructEvent(json, signature, configuration["Stripe:WebhookSecret"]);

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var paymentIntent = stripeEvent.Data.Object as Stripe.PaymentIntent;
                    await UpdateOrderStatusFromPaymentAsync(paymentIntent!.Id, true);
                }
                else if (stripeEvent.Type == "payment_intent.payment_failed")
                {
                    var paymentIntent = stripeEvent.Data.Object as Stripe.PaymentIntent;
                    await UpdateOrderStatusFromPaymentAsync(paymentIntent!.Id, false);
                }

                return true;
            }
            catch (Stripe.StripeException)
            {
                return false;
            }
        }

        private async Task UpdateOrderStatusFromPaymentAsync(string paymentIntentId, bool isSuccess)
        {
            var orderRepo = unitOfWork.GetRepository<Order, Guid>();
            var spec = new E_commerce.Services.Specifications.OrderWithPaymentIntentSpecifications(paymentIntentId);
            var order = await orderRepo.GetById(spec);

            if (order != null)
            {
                order.Status = isSuccess ? OrderStatus.PaymentReceived : OrderStatus.PaymentFailed;
                orderRepo.Update(order);
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}
