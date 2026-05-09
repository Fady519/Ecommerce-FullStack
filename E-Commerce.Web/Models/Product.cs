using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Web.Models
{
    public class Product
    {
        [Required(ErrorMessage ="Id Is Reqired")]
        public int Id { get; set; }

        [MaxLength(3, ErrorMessage = "Max Length Is 3 Chars")]
        public string Name { get; set; }


    }
}
