using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Model
{
    public class Team
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public string? LeaderId { get; set; }
        [ForeignKey(nameof(LeaderId))]
        public AppUser? Leader { get; set; }

        public IList<AppUser>? Members { get; set; }
    }
}
