namespace Inventory.Model.Entity
{
    public class Comment
    {
        public int Id { get; set; }
        public DateTime CommentAt { get; set; }
        public string? CommentBy { get; set; }
        public bool? IsReject { get; set; }
        public string? Message { get; set; }
    }
}
