using System.ComponentModel.DataAnnotations;

namespace Inventory.Repository.Model
{
    public class OrderEntity : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? CompleteDate { get; set; }
        public IList<OrderInfoEntity>? History { get; set; }
    }
}
