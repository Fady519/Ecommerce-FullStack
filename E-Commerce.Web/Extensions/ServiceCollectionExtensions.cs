using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using E_commerce.Domain.Contracts;
using E_commerce.Domain.Entities.IdentityModule;
using E_commerce.Persistence.Data.DataSeed;
using E_commerce.Persistence.Data.DbContexts;
using E_commerce.Persistence.Repositories;
using E_commerce.Presentation;
using E_commerce.Presentation.Controllers;
using E_commerce.Services;
using E_commerce.Services.Mapping;
using E_commerce.ServicesAbstraction;
using E_Commerce.Web.Factories;
using E_Commerce.Web.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace E_Commerce.Web.Extensions
{
    /// <summary>Groups the service registrations so Program.cs reads like a table of contents.</summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>Name of the CORS policy that lets the React storefront call the API.</summary>
        public const string FrontendCorsPolicy = "Frontend";

        /// <summary>The database (one DbContext for everything), repositories and the seeders.</summary>
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StoreDbContext>(options =>
                options.UseSqlServer(RequiredConnectionString(configuration, "DefaultConnection")));

            services.AddKeyedScoped<IDataInitializer, DataInitializer>("Default");
            services.AddKeyedScoped<IDataInitializer, IdentityDataInitializer>("Identity");
            services.Configure<AdminUserOptions>(configuration.GetSection(AdminUserOptions.SectionName));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Baskets now live in SQL Server too (they used to need a Redis server).
            services.AddScoped<IBasketRepository, BasketRepository>();

            return services;
        }

        /// <summary>Business services (the "Services" layer).</summary>
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IPictureUrlBuilder, RequestPictureUrlBuilder>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }

        /// <summary>ASP.NET Identity (users + roles), JWT bearer authentication, policies and login rate limiting.</summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityCore<ApplicationUser>(options =>
                {
                    // Password: 8+ characters with upper case, lower case and a digit (symbols optional).
                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;

                    // The email is the login, so two accounts can't share one.
                    options.User.RequireUniqueEmail = true;

                    // 5 wrong passwords → locked for 10 minutes (AuthenticationService counts the failures).
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                    options.Lockout.AllowedForNewUsers = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<StoreDbContext>();

            // The signing key is a secret: it lives in user-secrets (local) or environment variables (server),
            // never in appsettings.json. A short key would make tokens easy to forge, so we refuse to start.
            var secretKey = configuration["JWTOptions:SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
                throw new InvalidOperationException(
                    "JWTOptions:SecretKey is missing or shorter than 32 characters. " +
                    "Set it with: dotnet user-secrets set \"JWTOptions:SecretKey\" \"<a long random value>\" --project E-Commerce.Web");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWTOptions:Issuer"],
                    ValidAudience = configuration["JWTOptions:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    // The default tolerance is 5 minutes; 1 is enough for small clock differences.
                    ClockSkew = TimeSpan.FromMinutes(1),
                };
            });

            // Policies: controllers ask for a name, the rule lives here.
            services.AddAuthorizationBuilder()
                .AddPolicy(AuthPolicies.AdminOnly, policy => policy.RequireRole(AppRoles.Admin));

            // Login/register/emailExists: 10 requests per minute per IP. Together with the lockout this makes
            // password guessing impractical. Over the limit → 429 with a Problem Details body and Retry-After.
            var permitPerMinute = configuration.GetValue("RateLimiting:AuthPermitPerMinute", 10);
            services.AddRateLimiter(options =>
            {
                options.AddPolicy(AuthPolicies.AuthRateLimit, context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = permitPerMinute,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                        }));

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, cancellationToken) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                        context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();

                    var problemDetails = context.HttpContext.RequestServices.GetRequiredService<IProblemDetailsService>();
                    await problemDetails.WriteAsync(new ProblemDetailsContext
                    {
                        HttpContext = context.HttpContext,
                        ProblemDetails = new ProblemDetails
                        {
                            Status = StatusCodes.Status429TooManyRequests,
                            Title = "Too many requests",
                            Detail = "Too many attempts from your network. Please wait a minute and try again.",
                        },
                    });
                };
            });

            return services;
        }

        /// <summary>Controllers, Problem Details errors, CORS and Swagger.</summary>
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.Configure<ApiBehaviorOptions>(options =>
                options.InvalidModelStateResponseFactory = ApiResponseFactory.GenerateApiValidationError);

            // Every error (404 route, 401, 500...) is answered in the same RFC 9457 Problem Details shape.
            services.AddProblemDetails(options =>
                options.CustomizeProblemDetails = context =>
                    context.ProblemDetails.Instance ??= $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");
            services.AddExceptionHandler<GlobalExceptionHandler>();

            // Catalog responses are kept in memory for 5 minutes (one copy per query string: page, sort, brand...).
            // The "catalog" tag lets the admin endpoints clear them as soon as a product changes.
            services.AddOutputCache(options =>
                options.AddPolicy(ProductsController.CatalogCachePolicy, policy => policy
                    .Expire(TimeSpan.FromMinutes(5))
                    .SetVaryByQuery("*")
                    .Tag("catalog")));

            // Browsers block calls from another origin (the React app) unless the API allows them.
            // Origins come from "Cors:AllowedOrigins" so the live site can be added without code changes.
            var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
            services.AddCors(options => options.AddPolicy(FrontendCorsPolicy, policy =>
                policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod()));

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "E-Commerce API",
                    Version = "v1",
                    Description = "REST API for an online store: catalog with filtering and paging, basket, checkout with "
                        + "delivery methods, orders, Stripe payments and JWT authentication. Errors use RFC 9457 Problem Details.",
                });

                var xmlFile = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                if (File.Exists(xmlFile))
                    options.IncludeXmlComments(xmlFile);

                // Adds the "Authorize" button: paste the token returned by the login endpoint.
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Paste the token returned by POST /api/Authentication/Login (without the word Bearer).",
                });
                options.OperationFilter<AuthResponsesOperationFilter>();
                options.SupportNonNullableReferenceTypes();
            });

            return services;
        }

        private static string RequiredConnectionString(IConfiguration configuration, string name) =>
            configuration.GetConnectionString(name)
            ?? throw new InvalidOperationException($"Connection string '{name}' is not configured (see appsettings.Development.json).");
    }
}
