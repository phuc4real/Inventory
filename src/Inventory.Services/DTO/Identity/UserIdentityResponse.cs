using Inventory.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Identity
{
    public class UserIdentityResponse
    {
        public string? UserId { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpireTime { get; set; }
    }

    public class IdentityObjectResponse : ObjectResponse<UserIdentityResponse> { }
}
