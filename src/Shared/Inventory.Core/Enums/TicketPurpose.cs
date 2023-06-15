using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Enums
{
    public enum TicketPurpose
    {
        [Description("Yeu cau xuat vat tu")]
        ExportItem = 1,
        [Description("Yeu cau doi vat tu")]
        ChangeItem = 2,
        [Description("Yeu cau sua chua vat tu")]
        FixItem = 3,
        [Description("Yeu cau nhap vat tu")]
        ImportItem = 4
    }
}
