using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.ViewModel
{
    public class TeamDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public AppUserDTO? Leader { get; set; }
    }

    public class TeamEditDTO
    {
        public string? Name { get; set; }
        public string? LeaderId { get; set; }
    }

    public class TeamWithMembersDTO : TeamDTO
    {
        public IList<AppUserDTO>? Members { get; set; }
    }
}
