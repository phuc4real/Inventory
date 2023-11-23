using Inventory.Core.Common;
using Inventory.Core.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Inventory.API.Extensions
{
    public static partial class ServicesExtensions
    {
        public static IServiceCollection AddControllerService(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Conventions
                    .Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            })
                //.AddJsonOptions(options =>
                //{
                //    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                //})
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = (errorContext) =>
                    {
                        var errors = errorContext.ModelState
                            .Where(m => m.Value!.Errors.Any())
                            .Select(m => new ResultMessage(
                                m.Key,
                                m.Value!.Errors.FirstOrDefault()!.ErrorMessage))
                            .ToList();
                        return new BadRequestObjectResult(errors);
                    };
                });

            return services;
        }
    }
}
