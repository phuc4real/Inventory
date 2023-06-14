using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace Inventory.API.RateLimits
{
    public class LimitRequestPerMinutesPolicy : IRateLimiterPolicy<string>
    {
        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected { get; }
            = (context, cancellationToken) => {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return new();
            };

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            return RateLimitPartition.GetFixedWindowLimiter(string.Empty, options => new()
            {
                PermitLimit = 1,
                Window = TimeSpan.FromMinutes(5)
            });
        }
    }
}
