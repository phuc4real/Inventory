using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Model
{
    public class Export : BaseModel
    {
        public string? Description { get; set; }

        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser? ForUser { get; set; }
    }
}
