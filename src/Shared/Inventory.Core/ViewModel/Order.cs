namespace Inventory.Core.ViewModel
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public AppUser? CreatedByUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public AppUser? UpdatedByUser { get; set; }

        public DateTime? CompleteDate { get; set; }
        public IList<OrderInfo>? History { get; set; }
    }
}
