using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Model
{
    public class Team : BaseModel
    {
        public string? Name { get; set; }
        public int TotalMember { get; set; }
        public string? PmId { get; set; }
        [ForeignKey(nameof(PmId))]
        public IdentityUser? ProjectManager { get; set; }
        public IList<AppUser>? Members { get; set; }
    }
}
