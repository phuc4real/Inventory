using Inventory.Core.Common;
using Inventory.Service.DTO.Order;
using Inventory.Service.DTO.Ticket;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Validation
{
    public class TicketValidation
    {
        public static ResultMessage Validate(TicketUpdateResquest entity)
        {
            if (entity == null)
            {
                return new ResultMessage("Error", "Invalid input data!");
            }

            if (entity.Title.IsNullOrEmpty())
            {
                return new ResultMessage("Error", "Ticket title is missing!");
            }

            if (entity.TicketTypeId <= 0)
            {
                return new ResultMessage("Error", "Ticket type is invalid!");
            }

            if (!entity.TicketEntries.Any())
            {
                return new ResultMessage("Error", "No item in the ticket!");
            }

            if (entity.TicketEntries.Sum(x => x.Quantity) == 0)
            {
                return new ResultMessage("Error", "Quantity of item is invalid!");
            }

            return new ResultMessage();
        }
    }
}
