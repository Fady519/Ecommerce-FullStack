using E_commerce.Shared.DTOs.BasketDTOs;

namespace E_commerce.ServicesAbstraction
{
    /// <summary><c>userId</c> = the caller from the token, or null for a guest.</summary>
    public interface IBasketService
    {


        Task<BasketDTO> GetBasketAsync(string id, string? userId);

        Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basket, string? userId);


        Task<bool> DeleteBasketAsync(string id, string? userId);

        /// <summary>The logged-in user's basket (e.g. on a new device after login).</summary>
        Task<BasketDTO> GetUserBasketAsync(string userId);

        /// <summary>Links a guest basket to the user right after login, merging any basket they already had.</summary>
        Task<BasketDTO> ClaimBasketAsync(string id, string userId);

    }
}
