namespace Inventory.Core.ViewModel
{
    public class ItemDetail : Item
    {
        public int InStock { get; set; }
        public int InUsing { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }
        public AppUsers? CreatedByUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public AppUsers? UpdatedByUser { get; set; }

        public IList<ExportDetail>? ExportDetails { get; set; }
    }
}
