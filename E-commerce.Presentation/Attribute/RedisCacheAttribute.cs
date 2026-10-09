using E_commerce.ServicesAbstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_commerce.Presentation.Attribute
{
    internal class RedisCacheAttribute : ActionFilterAttribute
    {
        private readonly int _durationMin;

        public RedisCacheAttribute(int DurationMin = 5)
        {
            _durationMin = DurationMin;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // get cache service from di container
            // check if cached data is existed
            // if exists => return cached data and skip executiong of endPoint
            // if not exist => execute the endPoint and store the result in cache if 200 ok success
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

            var cacheKey = CreateCacheKey(context.HttpContext.Request);

            var cacheValue = await cacheService.GetAsync(cacheKey);
            if (cacheValue != null)
            {
                context.Result = new ContentResult()
                {
                    Content = cacheValue,
                    ContentType = "application/Json",
                    StatusCode = StatusCodes.Status200OK
                };

                return;
            }

            var ExecutedContext = await next.Invoke();
            if(ExecutedContext.Result is OkObjectResult { Value: not null } result)
            {
                await cacheService.SetAsync(cacheKey, result.Value, TimeSpan.FromMinutes(_durationMin));

            }


           // return base.OnActionExecutionAsync(context, next);
        }


        private string CreateCacheKey(HttpRequest request)
        {
            StringBuilder Key = new StringBuilder();
            Key.Append(request.Path);
            foreach(var item in request.Query.OrderBy(x => x.Key)) 
                Key.Append($"|{item.Key} - {item.Value}");

            return Key.ToString();
        }


    }
}
