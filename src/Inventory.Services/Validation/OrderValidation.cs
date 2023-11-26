using Inventory.Core.Common;
using Inventory.Model.Entity;
using Inventory.Service.DTO.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Validation
{
    public class OrderValidation
    {
        public static ResultMessage Validate(OrderUpdateRequest entity)
        {
            if (entity == null)
            {
                return new ResultMessage("Error", "Invalid input data!");
            }

            if (!entity.OrderEntries.Any())
            {
                return new ResultMessage("Error", "No item in the order!");
            }

            if (entity.OrderEntries.Sum(x => x.Quantity) == 0)
            {
                return new ResultMessage("Error", "Quantity of item is invalid!");
            }

            if (entity.OrderEntries.Sum(x => x.MinPrice) == 0)
            {
                return new ResultMessage("Error", "Min Price of item is invalid!");
            }

            if (entity.OrderEntries.Sum(x => x.MaxPrice) == 0)
            {
                return new ResultMessage("Error", "Max Price of item is invalid!");
            }

            return new ResultMessage();
        }
    }
}
