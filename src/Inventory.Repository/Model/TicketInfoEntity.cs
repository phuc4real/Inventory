using Inventory.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Repository.Model
{
    public class TicketInfoEntity
    {
        [Key]
        public int Id { get; set; }
        public int?TicketId { get; set; }
        [ForeignKey(nameof(TicketId))]
        public TicketEntity? Ticket { get; set; }

        public TicketPurpose Purpose { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public TicketStatus Status { get; set; }
        public DecisionEntity? LeaderDecision { get; set; }
        public DecisionEntity? Decision { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime CloseAt { get; set; }
      
        public IList<ItemEntity>? Items { get; set; }
        public IList<TicketDetailEntity>? Details { get; set; }
    }
}
