using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.OrderModule;
using E_commerce.Domain.Entities.ProductModule;
using E_commerce.Services.Exceptions;
using E_commerce.Services.Mapping;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.BasketDTOs;

namespace E_commerce.Services
{
    public class BasketService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IPictureUrlBuilder pictures) : IBasketService
    {
        public async Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basket)
        {
            var entity = basket.ToEntity();

            // Every product must really exist (a foreign key would refuse it anyway, but with an ugly 500 error).
            var products = unitOfWork.GetRepository<Product, int>();
            foreach (var item in entity.Items)
            {
                _ = await products.GetById(item.ProductId) ?? throw new ProductNotFoundException(item.ProductId);
            }

            if (entity.DeliveryMethodId is int deliveryMethodId)
            {
                _ = await unitOfWork.GetRepository<DeliveryMethod, int>().GetById(deliveryMethodId)
                    ?? throw new DeliveryMethodNotFoundException(deliveryMethodId);
            }

            var saved = await basketRepository.CreateOrUpdateBasketAsync(entity);
            return saved.ToDto(pictures);
        }

        public Task<bool> DeleteBasketAsync(string id) => basketRepository.DeleteBasketAsync(id);

        public async Task<BasketDTO> GetBasketAsync(string id)
        {
            var basket = await basketRepository.GetBasketAsync(id) ?? throw new BasketNotFoundException(id);
            return basket.ToDto(pictures);
        }
    }
}
