using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace E_commerce.ServicesAbstraction
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile imageFile, string folderName);
        void DeleteImage(string imageUrl);
    }
}
