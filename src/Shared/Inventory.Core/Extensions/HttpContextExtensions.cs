using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Inventory.Core.Extensions
{
    public static class HttpContextExtensions
    {
        public static async Task<string> GetAccessToken(this HttpContext context)
        {
            string token;
            token = await context.GetTokenAsync("access_token");
            if (token == null) 
            { 
                context.Request.Headers.TryGetValue("Authorization", out var tokenFromHeaders);
                token = tokenFromHeaders.ToString().Replace("Bearer ", "");
            }
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
