using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Enums
{
    public enum InputRoles
    {
        [Description("Employee")]
        Employee = 1,
        [Description("Project Manager")]
        ProjectManager = 2,
        [Description("Inventory Manager")]
        InventoryManager = 3,
        [Description("Administrator")]
        Admin = 4,

    }
}
