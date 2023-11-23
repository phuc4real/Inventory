using Inventory.Core.Common;

namespace Inventory.Service.DTO.Category
{
    public class CategoryRequest : BaseRequest
    {
        public int? Id { get; set; }
    }

    public class CategoryUpdateRequest : CategoryRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
