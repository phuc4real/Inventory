using Inventory.Service.Common;
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
        public string? Description { get; set; }
    }

    public class CategoryObjectResponse : ObjectResponse<CategoryResponse> { }

    public class CategoryPaginationResponse : PaginationResponse<CategoryResponse> { }
}
