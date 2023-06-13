using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Enums
{
    public enum OrderStatus
    {
        [Description("Dang cho")]
        Pending = 1,
        [Description("Dang xu ly")]
        Processing = 2,
        [Description("Hoan thanh")]
        Done = 3,
        [Description("Da huy")]
        Cancel = 4
    }
}
