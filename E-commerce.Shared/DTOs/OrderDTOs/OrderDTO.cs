using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Shared.DTOs.OrderDTOs
{
    public record OrderDTO(string BasketId , int DeliveryMethodId , AddressDTO AddressDTO, string PaymentMethod = "Card");
}
