namespace Inventory.Core.ViewModel
{
    public class Export
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public AppUser? ForUser { get; set; }

        public DateTime CreatedDate { get; set; }
        public AppUser? CreatedByUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public AppUser? UpdatedByUser { get; set; }

        public IList<ExportDetail>? Details { get; set; }
    }
}
