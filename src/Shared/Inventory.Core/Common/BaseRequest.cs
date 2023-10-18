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
        private string? _query;

        public void SetContext(HttpContext context)
        {
            _query = context.Request.QueryString.ToString();

            if (context.Request.Headers.TryGetValue("X-User-Id", out var userId))
            {
                _userContext = userId;
            }
            else
            {
                throw new Exception("Cannot get x-user-id header");
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
    }
}
