using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Enums
{
    public enum TicketDetailType
    {
        [Description("Xuat moi")]
        New = 1,
        [Description("Doi cu")]
        ChangeFrom = 2,
        [Description("Doi moi")]
        ChangeTo = 3,
        [Description("Sua chua")]
        Fix = 4
    }
}
