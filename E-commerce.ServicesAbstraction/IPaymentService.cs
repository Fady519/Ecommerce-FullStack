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

        Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string BasketId);

    }
}
