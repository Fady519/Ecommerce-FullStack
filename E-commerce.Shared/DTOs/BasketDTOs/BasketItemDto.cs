using System.ComponentModel.DataAnnotations;

namespace E_commerce.Shared.DTOs.BasketDTOs
{
    /// <summary>
    /// One basket line. The client only needs to send <c>Id</c> (the product id) and <c>Quantity</c>;
    /// name, picture and price are filled by the API from the catalog (anything the client sends there is ignored,
    /// so nobody can buy a 1200 EGP jacket for 1 EGP by editing the request).
    /// </summary>
    public record BasketItemDto(
        int Id,
        [Range(1, 100)] int Quantity,
        string ProductName = "",
        string PictureUrl = "",
        decimal Price = 0);
}
