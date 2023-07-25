using AutoMapper;
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
        private readonly IOrderInfoRepository _orderInfo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IItemService _itemService;
        private readonly ITokenService _tokenService;

        public OrderService(
            IOrderRepository order,
            IOrderInfoRepository orderInfo,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IItemService itemService,
            ITokenService tokenService)
        {
            _order = order;
            _orderInfo = orderInfo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _itemService = itemService;
            _tokenService = tokenService;
        }

        public async Task<ResultResponse<IEnumerable<Order>>> GetList()
        {
            ResultResponse<IEnumerable<Order>> response = new();
            var orders = await _order.GetList();

            if (orders.Any())
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<Order>>(orders);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
            }
            return response;
        }

        public async Task<PaginationResponse<Order>> GetPagination(PaginationRequest request)
        {
            PaginationResponse<Order> response = new()
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
                response.Data = _mapper.Map<IEnumerable<Order>>(orders.Data);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
            }

            return response;
        }

        public async Task<ResultResponse<Order>> Create(string token, UpdateOrderInfo dto)
        {
            ResultResponse<Order> response = new();
            List<OrderDetailEntity> orderDetails = new();

            var userId = _tokenService.GetuserId(token);
            long minTotal = 0;
            long maxTotal = 0;

            var res = await _itemService.Exists(dto.Details!
                                                .Select(x => x.ItemId).ToList());

            if (res.Status != ResponseCode.Success)
            {
                response.Message = res.Message;
                response.Status = ResponseCode.NotFound;
                return response;
            }

            foreach (var detail in dto.Details!)
            {
                bool isMinDetailTotalValid = (detail.MinPrice * detail.Quantity) == detail.MinTotal;
                bool isMaxDetailTotalValid = (detail.MaxPrice * detail.Quantity) == detail.MaxTotal;

                if (isMinDetailTotalValid && isMaxDetailTotalValid)
                {
                    response.Status = ResponseCode.BadRequest;
                    response.Message = new("Order Detail", "Detail total not match!");
                    return response;
                }

                minTotal += detail.MinTotal;
                maxTotal += detail.MaxTotal;
                orderDetails.Add(_mapper.Map<OrderDetailEntity>(detail));
            }

            if (minTotal != dto.MinTotal && maxTotal != dto.MaxTotal)
            {
                response.Status = ResponseCode.BadRequest;
                response.Message = new("Order", "Order total not match!");
                return response;
            }

            OrderInfoEntity orderInfo = new()
            {
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                MinTotal = dto.MinTotal,
                MaxTotal = dto.MaxTotal,
                Description = dto.Description,
                Details = orderDetails
            };

            OrderEntity order = new()
            {
                CreatedDate = DateTime.UtcNow,
                CreatedById = userId,
                UpdatedDate = DateTime.UtcNow,
                UpdatedById = userId,
                History = new List<OrderInfoEntity>()
            };

            order.History.Add(orderInfo);

            await _order.AddAsync(order);
            await _unitOfWork.SaveAsync();

            response.Data = _mapper.Map<Order>(order);
            response.Status = ResponseCode.Created;
            response.Message = new("Order", "Order created!");
            return response;
        }

        public async Task<ResultResponse<Order>> GetById(int id)
        {
            ResultResponse<Order> response = new();

            var order = await _order.GetById(id);

            if (order != null)
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<Order>(order);
            }
            else
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Order", "Order not found!");
            }

            return response;
        }

        public async Task<ResultResponse> UpdateStatus(string token, int id)
        {
            ResultResponse response = new();

            var userId = _tokenService.GetuserId(token);

            var order = await _order.GetById(id);
            if (order == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Order", "Order not found!");
            }
            else
            {
                var orderInfo = order.History!.OrderByDescending(x => x.CreatedAt).First();

                switch (orderInfo.Status)
                {
                    case OrderStatus.Pending:
                        {
                            orderInfo.Status++;
                            order.UpdatedDate = DateTime.UtcNow;
                            order.UpdatedById = userId;

                            _orderInfo.Update(orderInfo);
                            _order.Update(order);
                            await _unitOfWork.SaveAsync();

                            response.Status = ResponseCode.Success;
                            response.Message = new("Order", "Order status changed!");
                            break;
                        }
                    case OrderStatus.Processing:
                        {
                            orderInfo.Status++;
                            order.CompleteDate = DateTime.UtcNow;
                            order.UpdatedDate = DateTime.UtcNow;
                            order.UpdatedById = userId;

                            var res = await _itemService.Order(orderInfo.Details!.ToList());
                            if (res.Status != ResponseCode.Success) return res;

                            _orderInfo.Update(orderInfo);
                            _order.Update(order);
                            await _unitOfWork.SaveAsync();

                            response.Status = ResponseCode.Success;
                            response.Message = new("Order", "Order status changed!");
                            break;
                        }
                    default:
                        {
                            response.Status = ResponseCode.BadRequest;
                            response.Message = new("Order", "Order status cannot change!");
                            break;
                        }
                };
            }
            return response;
        }

        public async Task<ResultResponse> Cancel(string token, int id)
        {
            ResultResponse response = new();

            var userId = _tokenService.GetuserId(token);

            var order = await _order.GetById(id);
            if (order == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Order", "Order not found!");

            }
            else
            {
                var orderInfo = order.History!.OrderByDescending(x => x.CreatedAt).First();

                if (orderInfo.Status == OrderStatus.Done || orderInfo.Status == OrderStatus.Cancel)
                {
                    response.Status = ResponseCode.BadRequest;
                    response.Message = new("Order", "Order already cancelled!");
                }
                else
                {
                    orderInfo.Status = OrderStatus.Cancel;
                    order.UpdatedDate = DateTime.UtcNow;
                    order.UpdatedById = userId;

                    _orderInfo.Update(orderInfo);
                    _order.Update(order);
                    await _unitOfWork.SaveAsync();

                    response.Status = ResponseCode.Success;
                    response.Message = new("Order", "Order canceled!");
                }
            }
            return response;
        }

        public async Task<ResultResponse> Decide(string token, int id, UpdateDecision decision)
        {
            ResultResponse response = new();

            var userId = _tokenService.GetuserId(token);
            var order = await _order.GetById(id);

            if (order == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Order", "Order not found!");
            }
            else
            {
                var orderInfo = order.History!.OrderByDescending(x => x.CreatedAt).First();

                orderInfo.Decision = new()
                {
                    Status = decision.Status,
                    ById = userId,
                    Date = DateTime.UtcNow,
                    Message = decision.Message
                };

                order.UpdatedDate = DateTime.UtcNow;
                order.UpdatedById = userId;

                _orderInfo.Update(orderInfo);
                _order.Update(order);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseCode.Success;
                response.Message = new("Decision", "Success");
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ResponseMessage>>> GetCountByMonth()
        {
            ResultResponse<IEnumerable<ResponseMessage>> response = new()
            {
                Status = ResponseCode.Success,
                Data = await _order.GetCount()
            };

            return response;
        }
    }
}
