using E_commerce.ServicesAbstraction;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile, string folderName)
        {
            if (imageFile == null || imageFile.Length == 0)
                return string.Empty;

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return Path.Combine(folderName, uniqueFileName).Replace("\\", "/");
        }

        /// <summary>
        /// Deletes a picture, but only one that <see cref="SaveImageAsync"/> uploaded:
        /// "{folder}/{guid}_{name}" directly under wwwroot. Seed pictures (images/products/...),
        /// external URLs and anything else are left alone.
        /// </summary>
        public void DeleteImage(string imageUrl)
        {
            if (!IsUploadedImage(imageUrl)) return;

            var webRoot = Path.GetFullPath(_webHostEnvironment.WebRootPath);
            var filePath = Path.GetFullPath(Path.Combine(webRoot, imageUrl.Replace('/', Path.DirectorySeparatorChar)));
            if (!filePath.StartsWith(webRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) return;

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static bool IsUploadedImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl)) return false;

            var segments = imageUrl.Replace('\\', '/').Split('/');
            if (segments.Length != 2) return false;

            var folder = segments[0];
            var fileName = segments[1];
            if (folder.Length == 0 || folder.Equals("images", StringComparison.OrdinalIgnoreCase) || folder.Contains(':')) return false;

            // SaveImageAsync names every file "{Guid}_{original name}"; seed pictures never look like that.
            const int GuidLength = 36;
            return fileName.Length > GuidLength + 1
                && fileName[GuidLength] == '_'
                && Guid.TryParseExact(fileName[..GuidLength], "D", out _);
        }
    }
}
