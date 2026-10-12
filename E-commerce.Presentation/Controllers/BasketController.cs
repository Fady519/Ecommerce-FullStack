using E_commerce.ServicesAbstraction;
using E_commerce.Shared.DTOs.BasketDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.Presentation.Controllers
{

    /// <summary>
    /// Public: guests shop with a random basket id kept by the browser. If a token is sent, the basket becomes
    /// the user's, and from then on only that user can read or change it (others get 404).
    /// </summary>
    public class BasketController : ApiBaseController
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService) 
        {
            _basketService = basketService;
        }

        // GET 
        [HttpGet]
        [HttpGet("{id}")]
        public async Task<ActionResult<BasketDTO>> GetBasket(string id)
        {
            var Basket = await _basketService.GetBasketAsync(id, CurrentUserId);
            return Ok(Basket);
        }

        [HttpPost]
        public async Task<ActionResult<BasketDTO>> CreateOrUpdateBasket(BasketDTO basket)
        {
            var Basket = await _basketService.CreateOrUpdateBasketAsync(basket, CurrentUserId);
            return Ok(Basket);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBasket(string id)
        {
            var Result = await _basketService.DeleteBasketAsync(id, CurrentUserId);
            return Ok(Result);
        }

        /// <summary>The logged-in user's basket, e.g. after logging in on a new device (404 if they have none).</summary>
        [Authorize]
        [HttpGet("mine")]
        public async Task<ActionResult<BasketDTO>> GetMyBasket()
        {
            return Ok(await _basketService.GetUserBasketAsync(RequiredUserId));
        }

        /// <summary>
        /// Call right after login with the guest basket id: the basket becomes the user's, and a basket they
        /// already had (other device, older visit) is merged into it. Returns the single resulting basket.
        /// </summary>
        [Authorize]
        [HttpPost("{id}/claim")]
        public async Task<ActionResult<BasketDTO>> ClaimBasket(string id)
        {
            return Ok(await _basketService.ClaimBasketAsync(id, RequiredUserId));
        }

    }
}
