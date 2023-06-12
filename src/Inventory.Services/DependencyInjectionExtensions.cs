using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Repository.Repositories;
using Inventory.Services.IServices;
using Inventory.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Services
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(
                    configuration.GetConnectionString("Inventory"))
                );

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

        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICatalogRepository, CatalogRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DependencyInjectionExtensions).Assembly);
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICatalogServices,CatalogService>();
            services.AddScoped<ITeamServices,TeamService>();

            return services;
        }
    }
}
