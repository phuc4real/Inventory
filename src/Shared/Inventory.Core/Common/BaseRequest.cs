using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Common
{
    public class BaseRequest
    {
        private string? _userContext;
        private string? _accessToken;
        private string? _query;
        private string? _refreshToken;

        public void SetContext(HttpContext context)
        {
            _query = context.Request.QueryString.ToString();

            if (context.Request.Headers.TryGetValue("X-User-Id", out var userId))
            {
                _userContext = userId;
            }

            if (context.Request.Headers.TryGetValue("Authorization", out var accessToken))
            {
                _accessToken = accessToken.ToString().Replace("Bearer ", "");
            }

            if (context.Request.Headers.TryGetValue("X-Token-Refresh", out var refreshToken))
            {
                _refreshToken = refreshToken;
            }
        }

        public string? GetUserContext()
        {
            return _userContext;
        }

        public string? GetQueryString()
        {
            return _query;
        }

        public string? GetToken()
        {
            return _accessToken;
        }

        public (string?, string?) GetFullToken()
        {
            return (_accessToken, _refreshToken);
        }
    }
}
