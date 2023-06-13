using Inventory.Core.Enums;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.IServices
{
    public interface IOrderService
    {
        Task<ResultResponse<IEnumerable<OrderDTO>>> GetAll();
        Task<ResultResponse<IEnumerable<OrderDTO>>> GetOrdersByItemId(Guid id);
        Task<ResultResponse<OrderDTO>> GetById(int id);
        Task<ResultResponse<OrderDTO>> CreateOrder(string jwtToken, OrderCreateDTO dto);
        Task<ResultResponse<OrderDTO>> UpdateStatus(int id);
        Task<ResultResponse<OrderDTO>> CancelOrder(int id);
    }
}
