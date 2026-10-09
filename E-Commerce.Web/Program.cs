using E_Commerce.Web.Extensions;

namespace E_Commerce.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add services to the container

            builder.Services
                .AddApiServices(builder.Configuration)
                .AddPersistence(builder.Configuration)
                .AddBusinessServices()
                .AddJwtAuthentication(builder.Configuration);

            #endregion

            var app = builder.Build();

            #region Database migration and seeding

            // One-off command (doesn't start the web server): dotnet run --project E-Commerce.Web -- --seed-demo
            if (args.Contains("--seed-demo"))
            {
                await app.SeedDemoDataAsync();
                return;
            }

            // On by default in Development only (appsettings.Development.json). On the server the database is
            // updated on purpose, not as a side effect of the app restarting.
            if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
                await app.MigrateAndSeedAsync();

            #endregion

            #region Configure the HTTP request pipeline

            app.UseExceptionHandler();
            // Empty error responses (unknown route 404, 401, 405...) get a Problem Details body too.
            app.UseStatusCodePages();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // Locally the React app calls http://localhost:5163; a redirect would break its CORS preflight.
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseCors(ServiceCollectionExtensions.FrontendCorsPolicy);

            app.UseAuthentication();  // 1) who are you?
            app.UseAuthorization();   // 2) are you allowed?

            // After CORS and auth: a cached catalog response still gets the right CORS headers.
            app.UseOutputCache();

            app.MapControllers();

            #endregion

            await app.RunAsync();
        }
    }
}
