namespace Inventory.Core.ViewModel
{
    public class OrderInfo
    {
        public int Id { get; set; }

        public string? Status { get; set; }
        public Decision? Decision { get; set; }

        public DateTime? CreatedAt { get; set; }

        public long MinTotal { get; set; }
        public long MaxTotal { get; set; }
        public string? Description { get; set; }

        public IList<OrderDetail>? Details { get; set; }
    }

    public class UpdateOrderInfo
    {
        public long MinTotal { get; set; }
        public long MaxTotal { get; set; }
        public string? Description { get; set; }

        public IList<UpdateOrderDetail>? Details { get; set; }
    }
}
