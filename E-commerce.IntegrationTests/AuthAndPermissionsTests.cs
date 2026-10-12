using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace E_commerce.IntegrationTests
{
    // Program is defined in E-Commerce.Web
    public class AuthAndPermissionsTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthAndPermissionsTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetDashboard_WithoutToken_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/admin/dashboard");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetOrders_WithoutToken_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/order");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetProducts_WithoutToken_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/products");

            // Assert
            // It should be OK because products are public
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
