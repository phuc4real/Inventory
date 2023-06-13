using Inventory.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Model
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public double OrderTotal { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public string? OrderBy { get; set; }
        [ForeignKey(nameof(OrderBy))]
        public AppUser? OrderByUser { get; set; }
        public DateTime CompleteDate { get; set; }

        public IList<Item>? Items { get; set; }
        public IList<OrderDetail>? Details { get; set; }
    }
}
