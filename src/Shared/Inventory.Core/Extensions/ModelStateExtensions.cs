using Inventory.Core.Response;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Extensions
{
    public static class ModelStateExtensions
    {
        public static List<ResponseMessage> GetErrorMessages(this ModelStateDictionary modelStateDictionary)
        {
            var error = modelStateDictionary.Where(m => m.Value!.Errors.Any())
                .Select(m => new ResponseMessage(
                    m.Key,
                    m.Value!.Errors.FirstOrDefault()!.ErrorMessage))
                .ToList();

            return error;
        }
    }
}
