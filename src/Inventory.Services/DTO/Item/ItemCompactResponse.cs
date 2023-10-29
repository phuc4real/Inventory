using Inventory.Service.Common;
using Inventory.Service.DTO.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Item
{
    public class ItemCompactResponse
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
    }
    public class ItemCompactObjectResponse : ObjectResponse<ItemCompactResponse> { }

}
