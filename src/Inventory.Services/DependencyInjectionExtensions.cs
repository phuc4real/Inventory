using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Repository.Repositories;
using Inventory.Services.IServices;
using Inventory.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;

namespace Inventory.Services
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(
                options =>
                {
                    //options.UseSqlServer(configuration.GetConnectionString("InventorySQLServerAzure"));
                    options.UseNpgsql(configuration.GetConnectionString("PostgresNeonCloud"));
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
            .AddTokenProvider("Inventory", typeof(DataProtectorTokenProvider<AppUser>))
            .AddSignInManager();

            return services;
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {

            try
            {
                services.AddSingleton<IConnectionMultiplexer>(options =>
                    ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisCloud")));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            return services;
        }
        
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICatalogRepository, CatalogRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IExportRepository, ExportRepository>();
            services.AddScoped<IReceiptRepository, ReceiptRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IExportDetailRepository, ExportDetailRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DependencyInjectionExtensions).Assembly);
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICatalogServices,CatalogService>();
            services.AddScoped<ITeamServices,TeamService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<IReceiptService, ReceiptService>();  
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IUsingItemService, UsingItemService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
