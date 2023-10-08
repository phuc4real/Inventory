using Inventory.Database.DbContext;
using Inventory.Model.Entity;
using Inventory.Repository;
using Inventory.Repository.Implement;
using Inventory.Service;
using Inventory.Service.Implement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;
using StackExchange.Redis;

namespace Inventory.API.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
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
            .AddTokenProvider("Inventory Indentity", typeof(DataProtectorTokenProvider<AppUser>))
            .AddSignInManager();

            return services;
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
        {

            try
            {
                services.AddSingleton<IConnectionMultiplexer>(options =>
                    ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            return services;
        }

        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<ICategoryRepository, CategoryRepository>();
            //services.AddScoped<IItemRepository, ItemRepository>();
            //services.AddScoped<IOrderRepository, OrderRepository>();
            //services.AddScoped<IOrderEntryRepository, OrderEntryRepository>();
            //services.AddScoped<IExportRepository, ExportRepository>();
            //services.AddScoped<ITicketRepository, TicketRepository>();
            //services.AddScoped<ITicketRecordRepository, TicketRecordRepository>();
            //services.AddScoped<ExportEntryRepository, ExportEntryRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRepoWrapper, RepoWrapper>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DependencyInjectionExtensions).Assembly);
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IInUseService, InUseService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
