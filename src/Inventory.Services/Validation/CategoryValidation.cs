using Inventory.Core.Common;
using Inventory.Model.Entity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Validation
{
    public class CategoryValidation
    {
        public static ResultMessage Validate(Category entity)
        {
            if (entity == null)
            {
                return new ResultMessage("Error", "Invalid input data!");
            }

            if (entity.Name.IsNullOrEmpty())
            {
                return new ResultMessage("Error", "Category name is null!");
            }

            return new ResultMessage();
        }
    }
}
