using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Inventory.Repository.Model
{
    public class ItemEntity : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int InStock { get; set; }
        public int InUsing { get; set; }

        public int CatalogId { get; set; }
        [ForeignKey(nameof(CatalogId))]
        public CatalogEntity? Catalog { get; set; }

        public bool IsDeleted { get; set; }

        public IList<OrderInfoEntity>? OrderInfo { get; set; }
        public IList<OrderDetailEntity>? OrderDetails { get; set; }

        public IList<ExportEntity>? Exports { get; set; }
        public IList<ExportDetailEntity>? ExportDetails { get; set; }

        public IList<TicketInfoEntity>? TicketInfo { get; set; }
        public IList<TicketDetailEntity>? TicketDetails { get; set; }
    }
}
