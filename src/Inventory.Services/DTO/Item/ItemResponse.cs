using Inventory.Service.Common;
using Inventory.Service.DTO.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Item
{
    public class ItemResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int Unit { get; set; }
        public int UseUnit { get; set; }
        public CategoryResponse? Category { get; set; }


        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsInactive { get; set; }
        public DateTime InactiveAt { get; set; }
        public string? InactiveBy { get; set; }
    }

    public class ItemObjectResponse : ObjectResponse<ItemResponse> { }

    public class ItemPaginationResponse : PaginationResponse<ItemResponse> { }
}
