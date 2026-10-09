using System.Text.Json;
using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.OrderModule;
using E_commerce.Domain.Entities.ProductModule;
using E_commerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_commerce.Persistence.Data.DataSeed
{
    /// <summary>
    /// Fills an EMPTY catalog from the JSON files in JsonFiles/ (13 products, 6 brands, 4 types, 4 delivery methods).
    /// Runs on every start but only adds what is missing, so it never duplicates or overwrites the admin's edits.
    /// </summary>
    public class DataInitializer(StoreDbContext dbContext, ILogger<DataInitializer> logger) : IDataInitializer
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task InitializeAsync()
        {
            // No try/catch on purpose: if seeding fails the app must NOT start half-empty and hide the error
            // (the old code printed the exception to the console and carried on).

            if (!await dbContext.ProductBrands.AnyAsync())
            {
                var brands = await ReadAsync<NameRow>("brands.json");
                dbContext.ProductBrands.AddRange(brands.Select(B => new ProductBrand { Name = B.Name }));
            }

            if (!await dbContext.ProductTypes.AnyAsync())
            {
                var types = await ReadAsync<NameRow>("types.json");
                dbContext.ProductTypes.AddRange(types.Select(T => new ProductType { Name = T.Name }));
            }

            if (!await dbContext.DeliveryMethods.AnyAsync())
            {
                dbContext.DeliveryMethods.AddRange(await ReadAsync<DeliveryMethod>("delivery.json"));
            }

            // Brands and types must exist (with their real ids) before products can point at them.
            await dbContext.SaveChangesAsync();

            if (!await dbContext.Products.AnyAsync())
            {
                // Products name their brand and type ("ZARA", "Tops") instead of guessing ids like 1, 2, 3.
                var brandIds = await dbContext.ProductBrands.ToDictionaryAsync(B => B.Name, B => B.ID, StringComparer.OrdinalIgnoreCase);
                var typeIds = await dbContext.ProductTypes.ToDictionaryAsync(T => T.Name, T => T.ID, StringComparer.OrdinalIgnoreCase);

                foreach (var row in await ReadAsync<ProductRow>("products.json"))
                {
                    if (!brandIds.TryGetValue(row.Brand, out var brandId) || !typeIds.TryGetValue(row.Type, out var typeId))
                        throw new InvalidOperationException($"products.json: unknown brand '{row.Brand}' or type '{row.Type}' for '{row.Name}'.");

                    dbContext.Products.Add(new Product
                    {
                        Name = row.Name,
                        Description = row.Description,
                        Price = row.Price,
                        Stock = row.Stock,
                        PictureUrl = row.PictureUrl,
                        BrandId = brandId,
                        TypeId = typeId,
                    });
                }

                var added = await dbContext.SaveChangesAsync();
                logger.LogInformation("Catalog seeded: {Count} products.", added);
            }
        }

        /// <summary>
        /// Reads a seed file that is compiled into this dll (see the csproj: EmbeddedResource),
        /// so it works the same on the developer machine and on the server.
        /// </summary>
        private static async Task<List<T>> ReadAsync<T>(string fileName)
        {
            var resourceName = $"SeedData.{fileName}";
            await using var stream = typeof(DataInitializer).Assembly.GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Seed file '{fileName}' is not embedded in the Persistence dll.");

            return await JsonSerializer.DeserializeAsync<List<T>>(stream, JsonOptions)
                ?? throw new InvalidDataException($"Seed file '{fileName}' is empty.");
        }

        // Shapes of the JSON rows (they are not entities: products.json names its brand/type).
        private sealed record NameRow(string Name);
        private sealed record ProductRow(string Name, string Description, decimal Price, int Stock, string PictureUrl, string Brand, string Type);
    }
}
