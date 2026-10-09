using E_commerce.Shared.DTOs.BasketDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.ServicesAbstraction
{
    public interface IPaymentService
    {

        /// <summary><paramref name="userId"/> = the caller from the token; a basket owned by someone else is a 404.</summary>
        Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string BasketId, string userId);

    }
}
