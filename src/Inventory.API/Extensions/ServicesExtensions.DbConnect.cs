using Inventory.Database.DbContext;
using Inventory.Model.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;
using StackExchange.Redis;

namespace Inventory.API.Extensions
{
    public static partial class ServicesExtensions
    {
        public static IServiceCollection AddDatabaseService(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                services.AddDbContext<AppDbContext>(
                options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("DbConnect"));
                    options.ConfigureWarnings(builder =>
                        builder.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));
                });

                services.AddIdentity<AppUser, IdentityRole>(
                options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider("Inventory Identity", typeof(DataProtectorTokenProvider<AppUser>))
                .AddSignInManager();

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            return services;
        }

        public static IServiceCollection AddRedisCacheService(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                services.AddSingleton<IConnectionMultiplexer>(options => ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            return services;
        }
    }
}
