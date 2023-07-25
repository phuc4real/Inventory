using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Repository.Model
{
    public class AppUserEntity : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set;}

        public Guid? TeamId { get; set; }
        [ForeignKey(nameof(TeamId))]
        public TeamEntity? Team { get; set; }

        public DateTime? RefreshTokenExpireTime { get; set; }
    }
}
