using Microsoft.AspNetCore.Identity;

namespace Inventory.Model.Entity
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public DateTime? RefreshTokenExpireTime { get; set; }
    }
}
