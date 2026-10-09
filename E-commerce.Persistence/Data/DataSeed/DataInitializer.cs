using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities;
using E_commerce.Domain.Entities.OrderModule;
using E_commerce.Domain.Entities.ProductModule;
using E_commerce.Persistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_commerce.Persistence.Data.DataSeed
{
    public class DataInitializer : IDataInitializer
    {
        private readonly StoreDbContext _dbContext;

        public DataInitializer(StoreDbContext dbContext) 
        {
            _dbContext = dbContext;
        }


        public async Task InitializeAsync()
        {
            try
            {
                var HasProducts = await _dbContext.Products.AnyAsync();
                var HasBrands = await _dbContext.ProductBrands.AnyAsync();
                var HasTypes = await _dbContext.ProductTypes.AnyAsync();
                var HasDeliveryMethods = await _dbContext.Set<DeliveryMethod>().AnyAsync();

                if (HasProducts && HasBrands && HasTypes && HasDeliveryMethods) return;

                if (!HasBrands)
                {
                    await SeedDataFromJsonAsync<ProductBrand, int>("brands.json", _dbContext.ProductBrands);
                }

                if (!HasTypes)
                {
                    await SeedDataFromJsonAsync<ProductType, int>("types.json", _dbContext.ProductTypes);
                    await _dbContext.SaveChangesAsync();
                }

                if (!HasProducts)
                {
                    await SeedDataFromJsonAsync<Product, int>("products.json", _dbContext.Products);
                }

                if(!HasDeliveryMethods)
                {
                    await SeedDataFromJsonAsync<DeliveryMethod, int>("delivery.json", _dbContext.Set<DeliveryMethod>());
                }

                    await _dbContext.SaveChangesAsync();
                
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Data Seeding Faild {ex}");
            }

        }


        // Helper Method
        //E:\Back-End\08 ASP.NET Core Web API\Alia Tarek\Code\E-CommerceSolution\E-commerce.Persistence\Data\DataSeed\JsonFiles\
        private async Task SeedDataFromJsonAsync<T , TKey>(string fileName , DbSet<T> dbset) where T : BaseEntity<TKey>
        {
            var filePath = @"..\E-commerce.Persistence\Data\DataSeed\JsonFiles\" + fileName;

            if (!File.Exists(filePath)) throw new FileNotFoundException($"File {fileName} Is Not Exist");

            try
            {
                using var dataStream = File.OpenRead(filePath);

                var data = await JsonSerializer.DeserializeAsync<List<T>>(dataStream, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

                if(data != null)
                {
                    dbset.AddRange(data);
                }
            }

            catch(Exception ex) 
            {
                Console.WriteLine($"Error While Reading Json File : {ex}");
                return;
            }



        }

    }
}
