using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.ServicesAbstraction
{
    public interface ICacheService
    {

        Task<string?> GetAsync(string CacheKey);

        Task SetAsync(string CacheKey, object CacheValue, TimeSpan TimeToLive);


    }
}
