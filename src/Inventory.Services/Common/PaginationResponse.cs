using Inventory.Core.Enums;
using System.Text.Json.Serialization;

namespace Inventory.Service.Common
{
    public class PaginationResponse<T> : ListResponse<T> where T : class
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
    }
}
