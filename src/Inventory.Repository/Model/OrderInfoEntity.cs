using Inventory.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Repository.Model
{
    public class OrderInfoEntity
    {
        [Key]
        public int Id { get; set; }
        public int? OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public OrderEntity? Order { get; set; }

        public OrderStatus Status { get; set; }
        public DecisionEntity? Decision { get; set; }

        public DateTime? CreatedAt { get; set; }

        public long MinTotal { get; set; }
        public long MaxTotal { get; set; }
        public string? Description { get; set; }

        public IList<ItemEntity>? Items { get; set; }
        public IList<OrderDetailEntity>? Details { get; set; }
    }
}
