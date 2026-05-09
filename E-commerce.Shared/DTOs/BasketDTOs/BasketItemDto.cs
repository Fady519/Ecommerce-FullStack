using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Shared.DTOs.BasketDTOs
{
    public record BasketItemDto(int  id,
        string ProductName,
        string PictureUrl,
        [Range(1, double.MaxValue)]
        decimal Price,
        [Range(1, 100)]
        int Quantity);


}
