using Inventory.API.Filters;
using Microsoft.OpenApi.Models;

namespace Inventory.API.Extensions
{
    public static partial class ServicesExtensions
    {
        public static IServiceCollection AddSwaggerSerivce(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.OperationFilter<AuthorizationOperationFilter>();
                options.SwaggerDoc("v1", new OpenApiInfo { Title = configuration["PAppName"], Version = "v1" });

            });

            return services;
        }
    }
}
