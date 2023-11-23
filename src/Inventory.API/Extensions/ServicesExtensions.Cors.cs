namespace Inventory.API.Extensions
{
    public static partial class ServicesExtensions
    {
        public static IServiceCollection AddCorService(this IServiceCollection services, IConfiguration configuration)
        {

            var allowedCors = configuration.GetSection("AllowedCORS")
                                           .Get<string[]>() ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(allowedCors)
                          .WithExposedHeaders(new[] { "Location" })
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            return services;
        }
    }
}
