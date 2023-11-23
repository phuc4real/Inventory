namespace Inventory.Model.Entity
{
    public class Item : AuditLog
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int Unit { get; set; }
        public int UseUnit { get; set; }
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
    }
}
