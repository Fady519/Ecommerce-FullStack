using E_commerce.Services.Mapping;

namespace E_Commerce.Web.Infrastructure
{
    /// <summary>
    /// Builds picture URLs from the API's own public address.
    /// "Urls:PublicBaseUrl" wins when set (useful behind a proxy); otherwise the address of the current
    /// request is used, so the URLs are right on localhost, on any port, and on the live server without extra config.
    /// </summary>
    public sealed class RequestPictureUrlBuilder(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        : IPictureUrlBuilder
    {
        public string Build(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return string.Empty;

            // Already a full URL (e.g. an image hosted elsewhere): leave it alone.
            if (Uri.TryCreate(relativePath, UriKind.Absolute, out var absolute) && absolute.Scheme.StartsWith("http"))
                return relativePath;

            var baseUrl = configuration["Urls:PublicBaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                var request = httpContextAccessor.HttpContext?.Request;
                if (request is null)
                    return relativePath;
                baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            }

            return $"{baseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
        }
    }
}
