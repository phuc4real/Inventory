using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Inventory.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _order;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IItemRepository _item;
        private readonly ITokenService _tokenService;

        public OrderService(
            IOrderRepository order,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IItemRepository item,
            ITokenService tokenService)
        {
            _order = order;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _item = item;
            _tokenService = tokenService;
        }

        public async Task<ResultResponse<IEnumerable<OrderDTO>>> GetAll()
        {
            ResultResponse<IEnumerable<OrderDTO>> response = new();

            var orders = await _order.GetAllAsync();

            response.Status = ResponseStatus.STATUS_SUCCESS;
            response.Data = _mapper.Map<IEnumerable<OrderDTO>>(orders);

            return response;
        }


        public async Task<ResultResponse<OrderDTO>> CreateOrder(string jwtToken, OrderCreateDTO dto)
        {
            ResultResponse<OrderDTO> response = new()
            { Messages = new List<ResponseMessage>() };
            IList<OrderDetail> orderDetails = new List<OrderDetail>();

            var userid = _tokenService.GetUserId(jwtToken);

            foreach (var detail in dto.Details!)
            {
                var itemExists = await _item.AnyAsync(x => x.Id == detail.ItemId);

                if (!itemExists) 
                {
                    response.Status = ResponseStatus.STATUS_FAILURE;
                    response.Messages.Add(new ResponseMessage("Item", $"Item #{detail.ItemId} not exists!"));

                    return response;
                }

                orderDetails.Add(new OrderDetail()
                {
                    ItemId = detail.ItemId,
                    Price = detail.Price,
                    Quantity = detail.Quantity,
                    Total = detail.Total,
                });
            }


            Order order = new()
            {
                OrderTotal = dto.OrderTotal,
                Status = OrderStatus.Pending,
                Details = orderDetails,
                OrderBy = userid,
                OrderDate = DateTime.Now
            };
            
            await _order.AddAsync(order);
            await _unitOfWork.SaveAsync();

            response.Data = _mapper.Map<OrderDTO>(order);
            response.Status = ResponseStatus.STATUS_SUCCESS;
            response.Messages.Add(new ResponseMessage("Order", "Order created!"));
            return response;
        }

        public async Task<ResultResponse<OrderDTO>> GetById(int id)
        {
            ResultResponse<OrderDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var order = await _order.GetById(id);

            if(order != null)
            {
                response.Status= ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<OrderDTO>(order);
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Order", "Order not found!"));
            }

            return response;
        }

         public async Task<ResultResponse<OrderDTO>> UpdateStatus(int id)
        {
            ResultResponse<OrderDTO> response = new() { Messages = new List<ResponseMessage>() };

            var order = await _order.GetById(id);
            if (order == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Order", "Order not found!"));

            }
            else
            {
                switch (order.Status)
                {
                    case OrderStatus.Pending:
                    case OrderStatus.Processing:
                        {
                            order.Status++;
                            _order.Update(order);
                            await _unitOfWork.SaveAsync();

                            response.Status = ResponseStatus.STATUS_SUCCESS;
                            response.Data = _mapper.Map<OrderDTO>(order);

                            response.Messages.Add(new ResponseMessage("Order", "Order status changed!"));
                            break;
                        }
                    case OrderStatus.Done:
                        {
                            response.Status = ResponseStatus.STATUS_FAILURE;
                            response.Messages.Add(new ResponseMessage("Order", "Order is done!"));
                            break;
                        }
                    case OrderStatus.Cancel:
                        {
                            response.Status = ResponseStatus.STATUS_FAILURE;
                            response.Messages.Add(new ResponseMessage("Order", "Order is cancel!"));
                            break;
                        }
                };
            }

            return response;
        }

        public async Task<ResultResponse<OrderDTO>> CancelOrder(int id)
        {
            ResultResponse<OrderDTO> response = new() { Messages = new List<ResponseMessage>() };

            var order = await _order.GetById(id);
            if (order == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Order", "Order not found!"));

            }
            else
            {
                switch (order.Status)
                {
                    case OrderStatus.Pending:
                    case OrderStatus.Processing:
                        {
                            order.Status = OrderStatus.Cancel;
                            _order.Update(order);
                            await _unitOfWork.SaveAsync();

                            response.Status = ResponseStatus.STATUS_SUCCESS;
                            response.Data = _mapper.Map<OrderDTO>(order);

                            response.Messages.Add(new ResponseMessage("Order", "Order canceled!"));
                            break;
                        }
                    case OrderStatus.Done:
                        {
                            response.Status = ResponseStatus.STATUS_FAILURE;
                            response.Messages.Add(new ResponseMessage("Order", "Order is done"));
                            break;
                        }
                    case OrderStatus.Cancel:
                        {
                            response.Status = ResponseStatus.STATUS_FAILURE;
                            response.Messages.Add(new ResponseMessage("Order", "Order already cancel!"));
                            break;
                        }
                };
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<OrderDTO>>> GetOrdersByItemId(Guid id)
        {
            ResultResponse<IEnumerable<OrderDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var item = await _item.GetById(id);

            if (item == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Item", $"Item #{id} not exists!"));
            }
            else
            {
                var orders = await _order.OrdersByItem(item);

                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<OrderDTO>>(orders);
            }

            return response;
        }
    }
}
