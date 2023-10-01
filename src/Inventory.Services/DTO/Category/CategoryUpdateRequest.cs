using Inventory.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Category
{
    public class CategoryUpdateRequest : BaseRequest
    {
        public string? Name { get; set; }
    }
}
