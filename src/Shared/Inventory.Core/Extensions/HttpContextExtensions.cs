using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Inventory.Core.Extensions
{
    public static class HttpContextExtensions
    {
        public static async Task<string> GetAccessToken(this HttpContext context)
        {
            var token = await context.GetTokenAsync("access_token");
            return token!;
        }

        public static string GetRefreshToken(this HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("RefreshToken", out var token))
            {
                return token!;
            }

            return "";
        }
    }
}
