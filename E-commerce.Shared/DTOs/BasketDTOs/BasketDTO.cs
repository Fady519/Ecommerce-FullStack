using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Shared.DTOs.BasketDTOs
{

    public record BasketDTO(string Id , ICollection<BasketItemDto> Items ,
        string? ClientSecret , string? PaymentIntentId , int? DeliveryMethodId , decimal? ShippingPrice);


}
