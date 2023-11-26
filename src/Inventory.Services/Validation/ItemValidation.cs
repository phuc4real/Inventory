using Inventory.Core.Common;
using Inventory.Model.Entity;
using Inventory.Service.DTO.Item;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Validation
{
    public class ItemValidation
    {
        public static ResultMessage Validate(Item entity)
        {
            if (entity == null)
            {
                return new ResultMessage("Error", "Invalid input data!");
            }

            if (entity.Code.IsNullOrEmpty())
            {
                return new ResultMessage("Error", "Item code is null!");
            }

            if (entity.CategoryId <= 0)
            {
                return new ResultMessage("Error", "Category is invalid!");
            }

            if (entity.Name.IsNullOrEmpty())
            {
                return new ResultMessage("Error", "Item name is null!");
            }

            return new ResultMessage();
        }

    }
}
