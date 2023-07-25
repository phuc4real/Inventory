using System.ComponentModel.DataAnnotations;

namespace Inventory.Repository.Model
{
    public class TicketEntity : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? CloseDate { get; set; }
        public IList<TicketInfoEntity>? History { get; set; }
    }
}
