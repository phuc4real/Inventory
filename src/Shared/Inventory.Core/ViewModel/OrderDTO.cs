using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.ViewModel
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public double OrderTotal { get; set; }
        public DateTime OrderDate { get; set; }
        public string? Status { get; set; }
        public AppUserDTO? OrderByUser { get; set; }
        public DateTime CompleteDate { get; set; }
        public IList<OrderDetailDTO>? Details { get; set; }
    }

    public class OrderCreateDTO
    {
        public double OrderTotal { get; set; }
        public IList<OrderDetailCreateDTO>? Details { get; set; }
    }
}
