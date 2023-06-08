using Inventory.Repository.IRepository;
using Inventory.Repository.Repositories;
using Inventory.Services.IServices;
using Inventory.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddInventoryService(this IServiceCollection services)
        {
            //Add Repository
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICatalogRepository, CatalogRepository>();

            //Add Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICatalogServices,CatalogService>();

            return services;
        }
    }
}
