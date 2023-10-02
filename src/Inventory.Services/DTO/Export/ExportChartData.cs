using Inventory.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Export
{
    public class ExportChartData
    {
        public string? Month { get; set; }
        public int Value { get; set; }
    }

    public class ExportChartDataResponse : PaginationResponse<ExportChartData> { }
}
