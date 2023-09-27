using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository;
using Inventory.Repository.Model;
using Inventory.Service;
using Inventory.Service.Common;

namespace Inventory.Service.Implement
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _item;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly ICategoryServices _catalogService;

        public ItemService(
            IItemRepository item,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITokenService tokenService,
            ICategoryServices catalogService)
        {
            _item = item;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
            _catalogService = catalogService;
        }

        public async Task<ResultResponse<Item>> Create(string token, UpdateItem dto)
        {
            ResultResponse<Item> response = new();

            var userId = _tokenService.GetuserId(token);

            var catalogExists = await _catalogService.Any(dto.CatalogId);

            if (catalogExists.Status != ResponseCode.Success)
            {
                response.StatusCode = catalogExists.Status;
                response.Message = catalogExists.Message;
            }
            else
            {
                ItemEntity item = _mapper.Map<ItemEntity>(dto);
                item.CreatedById = userId;
                item.CreatedDate = DateTime.UtcNow;
                item.UpdatedById = userId;
                item.UpdatedDate = DateTime.UtcNow;

                await _item.AddAsync(item);
                await _unitOfWork.SaveAsync();

                response.Data = _mapper.Map<Item>(item);

                response.Message = new("Item", $"Item created!");
                response.StatusCode = ResponseCode.Success;
            }
            return response;
        }

        public async Task<ResultResponse<IEnumerable<Item>>> GetList(string? filter)
        {
            ResultResponse<IEnumerable<Item>> response = new();

            var items = await _item.GetList(filter!);

            if (items.Any())
            {
                response.StatusCode = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<Item>>(items);
            }
            else
                response.StatusCode = ResponseCode.NoContent;

            return response;
        }

        public async Task<PaginationResponse<Item>> GetPagination(PaginationRequest request)
        {
            PaginationResponse<Item> response = new()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            var items = await _item.GetPagination(request);

            if (items.Data!.Any())
            {
                response.TotalPages = items.TotalPages;
                response.TotalRecords = items.TotalRecords;
                response.StatusCode = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<Item>>(items.Data);
            }
            else
                response.StatusCode = ResponseCode.NoContent;

            return response;
        }

        public async Task<ResultResponse<ItemDetail>> GetById(Guid id)
        {
            ResultResponse<ItemDetail> response = new();

            var item = await _item.GetById(id);

            if (item == null)
            {
                response.StatusCode = ResponseCode.NotFound;
                response.Message = new("Item", "Item not found!");
            }
            else
            {
                response.StatusCode = ResponseCode.Success;
                response.Data = _mapper.Map<ItemDetail>(item);
            }

            return response;
        }

        public async Task<ResultResponse> Update(string token, Guid id, UpdateItem dto)
        {
            ResultResponse response = new();

            var userId = _tokenService.GetuserId(token);
            var item = await _item.GetById(id);

            if (item == null || item.IsDeleted)
            {
                response.StatusCode = ResponseCode.NotFound;
                response.Message = new("Item", "Item not found!");
            }
            else
            {
                item.Code = dto.Code;
                item.Name = dto.Name;
                item.Description = dto.Description;
                item.ImageUrl = dto.ImageUrl;
                item.CatalogId = dto.CatalogId;
                item.UpdatedById = userId;
                item.UpdatedDate = DateTime.UtcNow;

                _item.Update(item);
                await _unitOfWork.SaveAsync();

                response.StatusCode = ResponseCode.Success;
                response.Message = new("Item", "Item updated!");
            }

            return response;
        }

        public async Task<ResultResponse> Delete(string token, Guid id)
        {
            ResultResponse response = new();

            var userId = _tokenService.GetuserId(token);
            var item = await _item.GetById(id);

            if (item == null || item.IsDeleted)
            {
                response.StatusCode = ResponseCode.NotFound;
                response.Message = new("Item", "Item not found!");
            }
            else
            {
                item.IsDeleted = true;
                item.UpdatedById = userId;
                item.UpdatedDate = DateTime.UtcNow;

                _item.Update(item);
                await _unitOfWork.SaveAsync();

                response.StatusCode = ResponseCode.Success;
                response.Message = new("Item", "Item deleted!");
            }

            return response;
        }

        public async Task<ResultResponse> Export(List<ExportDetailEntity> details)
        {
            ResultResponse response = new();

            var res = await _item.GetRange(details.Select(x => x.ItemId).ToList());

            foreach (var item in res)
            {
                var i = details.Find(x => x.ItemId == item.Id);
                if (i!.Quantity > item.InStock)
                {
                    response.Message = new("Item", "Item #" + item.Name + " out of stock");
                    response.StatusCode = ResponseCode.BadRequest;
                    return response;
                }
                else
                {
                    item.InStock -= i.Quantity;
                    item.InUsing += i.Quantity;
                }
            }

            _item.UpdateRage(res.ToList());
            await _unitOfWork.SaveAsync();

            response.StatusCode = ResponseCode.Success;

            return response;
        }

        public async Task<ResultResponse> Exists(List<Guid> ids)
        {
            ResultResponse response = new();
            ResultMessage message = new("Item not exists", "");
            var res = await _item.GetRange(ids);

            if (res.Count() != ids.Count)
            {
                var listId = res.Select(x => x.Id)
                                .ToList();

                foreach (var id in ids)
                    if (!listId.Contains(id))
                        message.Value += $" {id}";

                response.Message = message;
                response.StatusCode = ResponseCode.NotFound;
            }
            else
            {
                response.StatusCode = ResponseCode.Success;
            }

            return response;
        }

        public async Task<ResultResponse> Order(List<OrderDetailEntity> details)
        {
            ResultResponse response = new();

            var res = await _item.GetRange(details.Select(x => x.ItemId).ToList());

            foreach (var item in res)
            {
                var i = details.Find(x => x.ItemId == item.Id);
                item.InStock += i!.Quantity;
            }

            _item.UpdateRage(res.ToList());
            await _unitOfWork.SaveAsync();

            response.StatusCode = ResponseCode.Success;

            return response;
        }
    }
}
