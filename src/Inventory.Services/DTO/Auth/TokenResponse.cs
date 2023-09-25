using Inventory.Service.Common.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Auth
{
    public class TokenResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpireTime { get; set; }
    }

    public class TokenObjectResponse : ObjectResponse<TokenResponse> { }
}
