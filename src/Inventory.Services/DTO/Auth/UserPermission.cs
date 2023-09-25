using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Auth
{
    public class UserPermission
    {
        public bool IsAdmin { get; set; }
        public bool IsSuperUser { get; set; }
    }
}
