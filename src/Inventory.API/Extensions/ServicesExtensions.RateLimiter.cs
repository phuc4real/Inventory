using Inventory.API.RateLimits;
using Inventory.Core.Common;
using System.Globalization;
using System.Threading.RateLimiting;

namespace Inventory.API.Extensions
{
    public static partial class ServicesExtensions
    {
        public static IServiceCollection AddRateLimiterService(this IServiceCollection services)
        {
            services.AddRateLimiter(option =>
            {
                option.AddPolicy<string, RefreshTokenLimitPolicy>("RefresshTokenLimit");
                option.OnRejected = (context, _) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                    }

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response
                        .WriteAsync(new ResultMessage("Too many request", "Please try again later!!").ToString(), cancellationToken: _);

                    return new ValueTask();
                };
                option.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                    PartitionedRateLimiter.Create<HttpContext, string>(HttpContext =>
                    {
                        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

                        return RateLimitPartition.GetFixedWindowLimiter(userAgent, opt => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 500,
                            Window = TimeSpan.FromMinutes(1)
                        });
                    }),
                    PartitionedRateLimiter.Create<HttpContext, string>(HttpContext =>
                    {
                        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
                        return RateLimitPartition.GetFixedWindowLimiter(userAgent, opt => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 25000,
                            Window = TimeSpan.FromMinutes(60)
                        });
                    })
                );
            });

            return services;
        }
    }
}
