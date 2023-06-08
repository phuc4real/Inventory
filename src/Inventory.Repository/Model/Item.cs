using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Model
{
    public class Item
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int InStock { get; set; }
        public int Used { get; set;}

        public int CatalogId { get; set; }
        [ForeignKey(nameof(CatalogId))]
        public Catalog? Catalog { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public AppUser? CreatedByUser { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }
        [ForeignKey(nameof(LastModifiedBy))]
        public AppUser? ModifiedByUser { get; set;}

        public bool IsDeleted { get; set; }

        //Order
        public IList<Order>? Orders { get; set; }
        public IList<OrderDetail>? OrderDetails { get; set; }

        //Export
        public IList<Export>? Exports { get; set; }
        public IList<ExportDetail>? ExportDetails { get; set; }

        //Receipt
        public IList<Receipt>? Receipts { get; set; }
        public IList<ReceiptDetail>? ReceiptDetails { get; set; }

        //Ticket
        public IList<Ticket>? Tickets { get; set; }
        public IList<TicketDetail>? TicketDetails { get; set; }
    }
}
