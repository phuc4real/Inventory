using Inventory.Service.Common.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Category
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class CategoryListResponse : ListResponse<CategoryResponse> { }

    public class CategoryPaginationResponse : PaginationResponse<CategoryResponse> { }
}
