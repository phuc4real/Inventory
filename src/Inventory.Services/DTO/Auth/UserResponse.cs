using Inventory.Service.Common.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Auth
{
    public class UserResponse
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public UserPermission? Permission { get; set; }
    }

    public class UserObjectResponse : ObjectResponse<UserResponse> { }

    public class UserListResponse : ListResponse<UserResponse> { }

}
