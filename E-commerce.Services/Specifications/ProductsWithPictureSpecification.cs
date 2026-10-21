using E_commerce.Domain.Entities.ProductModule;

namespace E_commerce.Services.Specifications
{
    /// <summary>
    /// Other products that use the same picture. Used before deleting an image file,
    /// so a picture shared by several products is never removed from under them.
    /// </summary>
    internal class ProductsWithPictureSpecification : BaseSpecifications<Product, int>
    {
        public ProductsWithPictureSpecification(string pictureUrl, int excludedProductId)
            : base(P => P.PictureUrl == pictureUrl && P.ID != excludedProductId)
        {
        }
    }
}
