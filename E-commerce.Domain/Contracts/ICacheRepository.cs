using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Domain.Contracts
{
    public interface ICacheRepository
    {

        Task<string?> GetAsync(string CacheKey);

        Task SetAsync(string CacheKey, string CacheValue , TimeSpan TimeToLive);


    }
}
