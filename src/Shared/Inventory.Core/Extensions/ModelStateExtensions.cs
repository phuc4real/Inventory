
using Inventory.Core.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inventory.Core.Extensions
{
    public static class ModelStateExtensions
    {
        public static List<ResultMessage> GetErrorMessages(this ModelStateDictionary modelStateDictionary)
        {
            var error = modelStateDictionary.Where(m => m.Value!.Errors.Any())
                .Select(m => new ResultMessage(
                    m.Key,
                    m.Value!.Errors.FirstOrDefault()!.ErrorMessage))
                .ToList();

            return error;
        }
    }
}
