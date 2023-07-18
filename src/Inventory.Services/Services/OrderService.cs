using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;

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

        public async Task<ResultResponse<IEnumerable<OrderDTO>>> GetList()
        {
            ResultResponse<IEnumerable<OrderDTO>> response = new();

            var orders = await _order.GetList();

            if (orders.Any())
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<OrderDTO>>(orders);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                //response.Message = new("Order", "There is no record");
            }

            return response;
        }


        public async Task<PaginationResponse<OrderDTO>> GetPagination(PaginationRequest request)
        {
            PaginationResponse<OrderDTO> response = new()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            var orders = await _order.GetPagination(request);

            if (orders.Data!.Any())
            {
                response.TotalRecords = orders.TotalRecords;
                response.TotalPages = orders.TotalPages;
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<OrderDTO>>(orders.Data);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                //response.Message = new("Order", "There is no record");
            }

            return response;
        }

        public async Task<ResultResponse<OrderDTO>> CreateOrder(string token, OrderCreateDTO dto)
        {
            ResultResponse<OrderDTO> response = new();
            IList<OrderDetail> orderDetails = new List<OrderDetail>();

            var userId = _tokenService.GetUserId(token);
            double total = 0;
            foreach (var detail in dto.Details!)
            {
                var itemExists = await _item.AnyAsync(x => x.Id == detail.ItemId);

                if (!itemExists) 
                {
                    response.Status = ResponseCode.NotFound;
                    response.Message = new("Item", $"Item #{detail.ItemId} not exists!");

                    return response;
                }
                double itemTotal = detail.Price * detail.Quantity;
                
                if(itemTotal != detail.Total) {
                    response.Status = ResponseCode.BadRequest;
                    response.Message = new("OrderDetail", "Item total not match!");
                    return response;
                }
                total += itemTotal;
                orderDetails.Add(_mapper.Map<OrderDetail>(detail));
            }

            if (total != dto.OrderTotal)
            {
                response.Status = ResponseCode.BadRequest;
                response.Message = new("Order", "Order total not match!");
                return response;
            }


            Order order = new()
            {
                OrderTotal = dto.OrderTotal,
                Status = OrderStatus.Pending,
                Details = orderDetails,
                OrderBy = userId,
                OrderDate = DateTime.UtcNow,
            };
            
            await _order.AddAsync(order);
            await _unitOfWork.SaveAsync();

            response.Data = _mapper.Map<OrderDTO>(order);
            response.Status = ResponseCode.Success;
            response.Message = new("Order", "Order created!");
            return response;
        }

        public async Task<ResultResponse<OrderDTO>> GetById(int id)
        {
            ResultResponse<OrderDTO> response = new();

            var order = await _order.GetById(id);

            if(order != null)
            {
                response.Status= ResponseCode.Success;
                response.Data = _mapper.Map<OrderDTO>(order);
            }
            else
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Order", "Order not found!");
            }

            return response;
        }

         public async Task<ResultResponse<OrderDTO>> UpdateOrderStatus(int id)
        {
            ResultResponse<OrderDTO> response = new();

            var order = await _order.GetById(id);
            if (order == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Order", "Order not found!");

            }
            else
            {
                switch (order.Status)
                {
                    case OrderStatus.Pending:
                        {
                            order.Status++;
                            _order.Update(order);
                            await _unitOfWork.SaveAsync();

                            response.Status = ResponseCode.Success;
                            response.Data = _mapper.Map<OrderDTO>(order);

                            response.Message = new("Order", "Order status changed!");
                            break;
                        }
                    case OrderStatus.Processing:
                        {
                            order.Status++;
                            order.CompleteDate = DateTime.UtcNow;
                            _order.Update(order);
                            await _unitOfWork.SaveAsync();

                            response.Status = ResponseCode.Success;
                            response.Data = _mapper.Map<OrderDTO>(order);

                            response.Message = new("Order", "Order status changed!");
                            break;
                        }
                    case OrderStatus.Done:
                        {
                            response.Status = ResponseCode.BadRequest;
                            response.Message = new("Order", "Order is done!");
                            break;
                        }
                    case OrderStatus.Cancel:
                        {
                            response.Status = ResponseCode.BadRequest;
                            response.Message = new("Order", "Order is cancel!");
                            break;
                        }
                };
            }

            return response;
        }

        public async Task<ResultResponse<OrderDTO>> CancelOrder(int id)
        {
            ResultResponse<OrderDTO> response = new();

            var order = await _order.GetById(id);
            if (order == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Order", "Order not found!");

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

                            response.Status = ResponseCode.Success;
                            response.Data = _mapper.Map<OrderDTO>(order);

                            response.Message = new("Order", "Order canceled!");
                            break;
                        }
                    case OrderStatus.Done:
                        {
                            response.Status = ResponseCode.BadRequest;
                            response.Message = new("Order", "Order is done");
                            break;
                        }
                    case OrderStatus.Cancel:
                        {
                            response.Status = ResponseCode.BadRequest;
                            response.Message = new("Order", "Order already cancelled!");
                            break;
                        }
                };
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ResponseMessage>>> GetCountByMonth()
        {
            ResultResponse<IEnumerable<ResponseMessage>> response = new();

            var result = await _order.GetCount();

            response.Status = ResponseCode.Success;
            response.Data = result;

            return response;
        }
    }
}
