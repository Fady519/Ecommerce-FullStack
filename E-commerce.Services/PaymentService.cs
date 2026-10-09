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

        public PaymentService(IBasketRepository basketRepository , IUnitOfWork unitOfWork , IConfiguration configuration)
        {
            this.basketRepository = basketRepository;
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
        }

        public async Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string BasketId)
        {

            // 1 ) Configure Stripe => Download Package "Stripe.Net"
            // 2 ) Get Basket Id
            
           

            StripeConfiguration.ApiKey = configuration["StripeSetings:SecretKey"];

            var Basket = await basketRepository.GetBasketAsync(BasketId);
            if(Basket == null)
            {
                throw new BasketNotFoundException(BasketId);
            }
            // 3 ) Amount
            var Product = unitOfWork.GetRepository<Domain.Entities.ProductModule.Product, int>();
            foreach(var item in Basket.Items)
            {
                var product = await Product.GetById(item.Id) ?? throw new ProductNotFoundException(item.Id);
                item.Price = product.Price;
            }

            // DeliverMethod

            var DeliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>().GetById(Basket.DeliveryMethodId.Value);

            Basket.ShippingPrice = DeliveryMethod.Price;

            var BasketAmount = (long) (Basket.Items.Sum(item => item.Quantity * item.Price) + DeliveryMethod.Price) * 100;

            // 4 ) Create PaymentIntent
            var PaymentService = new PaymentIntentService();
            if(Basket.PaymentIntentId == null)
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = BasketAmount,
                    Currency = "USD",
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

            await basketRepository.CreateOrUpdateBasketAsync(Basket);
            return Basket.ToDto();

        }
    }
}
