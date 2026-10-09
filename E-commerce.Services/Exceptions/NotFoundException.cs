using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Services.Exceptions
{
    public abstract class NotFoundException(string Message) : Exception(Message)
    {

        

    }

    public sealed class ProductNotFoundException(int id) : NotFoundException($"Product Not Found With {id}");


    public sealed class BasketNotFoundException(string id) : NotFoundException($"Basket With Id {id} Not Found");

    public sealed class DeliveryMethodNotFoundException(int? id) : NotFoundException(
        id is null ? "Choose a delivery method before paying." : $"Delivery method {id} was not found.");
}
