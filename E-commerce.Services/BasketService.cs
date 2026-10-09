using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.OrderModule;
using E_commerce.Domain.Entities.ProductModule;
using E_commerce.Services.Exceptions;
using E_commerce.Services.Mapping;
using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.BasketDTOs;

namespace E_commerce.Services
{
    /// <summary>
    /// Baskets work for guests (by a random id the browser keeps) and for logged-in users.
    /// <c>userId</c> is the caller from the token, or null for a guest. A basket that belongs to a user
    /// answers 404 to everybody else, as if it didn't exist (we don't even confirm the id is real).
    /// </summary>
    public class BasketService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IPictureUrlBuilder pictures) : IBasketService
    {
        public async Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basket, string? userId)
        {
            var current = await basketRepository.GetBasketAsync(basket.Id);
            if (current is not null && !current.CanBeUsedBy(userId))
                throw new BasketNotFoundException(basket.Id);

            var entity = basket.ToEntity();
            // A logged-in shopper owns the basket they fill (a guest basket stays without owner).
            entity.UserId = userId;

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

        public async Task<bool> DeleteBasketAsync(string id, string? userId)
        {
            var basket = await basketRepository.GetBasketAsync(id);
            if (basket is null) return false;
            if (!basket.CanBeUsedBy(userId)) throw new BasketNotFoundException(id);
            return await basketRepository.DeleteBasketAsync(id);
        }

        public async Task<BasketDTO> GetBasketAsync(string id, string? userId)
        {
            var basket = await basketRepository.GetBasketAsync(id);
            if (basket is null || !basket.CanBeUsedBy(userId)) throw new BasketNotFoundException(id);
            return basket.ToDto(pictures);
        }

        public async Task<BasketDTO> GetUserBasketAsync(string userId)
        {
            // Newest first: after a claim there is only one, but older data may still have more.
            var basket = (await basketRepository.GetUserBasketsAsync(userId)).FirstOrDefault()
                ?? throw new UserBasketNotFoundException();
            return basket.ToDto(pictures);
        }

        public async Task<BasketDTO> ClaimBasketAsync(string id, string userId)
        {
            var basket = await basketRepository.GetBasketAsync(id);
            if (basket is null || !basket.CanBeUsedBy(userId)) throw new BasketNotFoundException(id);

            // The user may already own a basket from another device or visit: fold it into this one,
            // so they end up with exactly one basket and nothing they added is lost.
            var others = (await basketRepository.GetUserBasketsAsync(userId)).Where(B => B.ID != id).ToList();
            foreach (var other in others)
                basket.MergeFrom(other);

            basket.UserId = userId;
            basket.UpdatedAt = DateTimeOffset.UtcNow;
            await unitOfWork.SaveChangesAsync(); // same DbContext as the repository, so the tracked basket is saved

            foreach (var other in others)
                await basketRepository.DeleteBasketAsync(other.ID);

            // Read it back so merged lines come with their product (name, price, picture).
            var merged = await basketRepository.GetBasketAsync(id) ?? throw new BasketNotFoundException(id);
            return merged.ToDto(pictures);
        }
    }
}
