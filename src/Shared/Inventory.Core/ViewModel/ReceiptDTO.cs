using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.ViewModel
{
    public class ReceiptDTO
    {
        public int Id { get; set; }
        public int ItemCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public AppUserDTO? CreatedByUser { get; set; }
        public IList<ReceiptDetailDTO>? Details { get; set; }
    }

    public class ReceiptCreateDTO
    {
        public int ItemCount { get; set; }
        public IList<ReceiptDetailCreateDTO>? Details { get; set; }
    }
}
