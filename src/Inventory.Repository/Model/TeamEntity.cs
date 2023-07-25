using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Repository.Model
{
    public class TeamEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public string? LeaderId { get; set; }
        [ForeignKey(nameof(LeaderId))]
        public AppUserEntity? Leader { get; set; }

        public IList<AppUserEntity>? Members { get; set; }
    }
}
