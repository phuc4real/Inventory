using Inventory.Core.Enums;

namespace Inventory.Core.ViewModel
{
    public class GrantRoleDTO
    {
        public string? UserId { get; set; }
        public InputRoles? Role { get; set; }
    }
}
