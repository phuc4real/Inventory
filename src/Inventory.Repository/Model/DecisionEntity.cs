using Inventory.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Repository.Model
{
    public class DecisionEntity
    {
        public DecisionStatus Status { get; set; }
        public DateTime Date { get; set; }

        public string? ById { get; set; }
        [ForeignKey(nameof(ById))]
        public AppUserEntity? ByUser { get; set; }

        public string? Message { get; set; }
    }
}
