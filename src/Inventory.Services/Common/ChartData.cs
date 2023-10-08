using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Common
{
    public class ChartData
    {
        public string? Month { get; set; }
        public int Value { get; set; }
    }

    public class ChartDataResponse : PaginationResponse<ChartData> { }
}
