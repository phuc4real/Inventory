using Inventory.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Repository.Model
{
    public class ExportEntity : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string? Description { get; set; }
        public ExportStatus Status { get; set; }
        public string? ForId { get; set; }
        [ForeignKey(nameof(ForId))]
        public AppUserEntity? ForUser { get; set; }

        public IList<ItemEntity>? Items { get; set; }
        public IList<ExportDetailEntity>? Details { get; set; }
    }
}
