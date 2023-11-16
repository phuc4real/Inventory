using Inventory.Repository;
using Inventory.Repository.Implement;
using Inventory.Service;
using Inventory.Service.Implement;
using Inventory.Service.MappingProfile;

namespace Inventory.API.Extensions
{
    public static partial class ServicesExtensions
    {


        public static IServiceCollection AddRepositoryService(this IServiceCollection services)
        {
            services.AddScoped<IRepoWrapper, RepoWrapper>();

            return services;
        }

        public static IServiceCollection AddAppService(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperConfig));
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IExportService, ExportService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<DbSeeder>();

            return services;
        }
    }
}
