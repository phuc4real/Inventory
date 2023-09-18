using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Repository.Model
{
    public class BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public string? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]   
        public AppUserEntity? CreatedByUser { get; set; }

        public DateTime UpdatedDate { get; set; }
        public string? UpdatedById { get; set; }
        [ForeignKey(nameof(UpdatedById))]
        public AppUserEntity? UpdatedByUser { get; set; }
    }
}
