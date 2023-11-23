using Inventory.Database.DbContext;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Inventory.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static WebApplication ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            try
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<AppDbContext>();
                Log.Information("Trying to connect Db & Get migration");
                if (!context.Database.GetPendingMigrations().Any())
                {
                    Log.Information("No migration needed!");
                }
                else
                {
                    Log.Information("Started migration!");
                    context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            return app;
        }

        public static WebApplication SeedingData(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                Log.Information("Started seeding data");
                DbSeeder.Initialize(context).Wait();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            return app;
        }
    }
}
