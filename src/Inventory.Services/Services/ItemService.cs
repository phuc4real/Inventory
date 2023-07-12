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
    public class ItemService : IItemService
    {
        private readonly IItemRepository _item;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly ICatalogRepository _catalog;

        public ItemService(
            IItemRepository item,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITokenService tokenService,
            ICatalogRepository catalog)
        {
            _item = item;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
            _catalog = catalog;
        }

        public async Task<ResultResponse<ItemDetailDTO>> CreateItem(string token, ItemEditDTO dto)
        {
            ResultResponse<ItemDetailDTO> response = new();

            var userId = _tokenService.GetUserId(token);

            if (!_catalog.AnyAsync(x=> x.Id == dto.CatalogId).Result)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Catalog", $"Catalog #{dto.CatalogId} not exist!");
            }
            else
            {
                Item item = _mapper.Map<Item>(dto);
                item.CreatedBy = userId;
                item.CreatedDate = DateTime.UtcNow;
                item.LastModifiedBy = userId;
                item.LastModifiedDate = DateTime.UtcNow;

                await _item.AddAsync(item);
                await _unitOfWork.SaveAsync();

                response.Data = _mapper.Map<ItemDetailDTO>(item);

                response.Message = new("Item", $"Item created!");
                response.Status = ResponseCode.Success;
            }
            return response;
        }

        public async Task<ResultResponse<ItemDetailDTO>> DeleteItem(string token, Guid id)
        {
            ResultResponse<ItemDetailDTO> response = new();

            var userId = _tokenService.GetUserId(token);
            var item = await _item.GetById(id);

            if (item == null || item.IsDeleted)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Item", "Item not found!");
            }
            else
            {
                item.IsDeleted = true;
                item.LastModifiedBy = userId;
                item.LastModifiedDate = DateTime.UtcNow;
                _item.Update(item);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseCode.Success;
                response.Message = new("Item", "Item deleted!");
            }

            return response;
        }

        public async Task<PaginationResponse<ItemDetailDTO>> GetPagination(PaginationRequest requestParams)
        {
            PaginationResponse<ItemDetailDTO> response = new()
            {
                PageIndex = requestParams.PageIndex,
                PageSize = requestParams.PageSize
            };

            var items = await _item.GetPagination(requestParams);

            if (items.Data!.Any())
            {
                response.TotalPages = items.TotalPages;
                response.TotalRecords = items.TotalRecords;
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<ItemDetailDTO>>(items.Data);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                response.Message = new("Item", "There is no record");
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ItemDetailDTO>>> GetList(string? name)
        {
            ResultResponse<IEnumerable<ItemDetailDTO>> response = new();

            var items = await _item.GetList(name);

            if (items.Any())
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<ItemDetailDTO>>(items);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                response.Message = new("Item", "There is no record");
            }

            return response;
        }

        public async Task<ResultResponse<ItemDetailDTO>> GetById(Guid id)
        {
            ResultResponse<ItemDetailDTO> response = new();

            var item = await _item.GetById(id);
            
            if (item == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Item", "Item not found!");
            }
            else
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<ItemDetailDTO>(item);
            }

            return response;
        }

        public async Task<ResultResponse<ItemDetailDTO>> UpdateItem(string token, Guid id, ItemEditDTO dto)
        {
            ResultResponse<ItemDetailDTO> response = new();

            var userid = _tokenService.GetUserId(token);
            var item = await _item.GetById(id);

            if (item == null || item.IsDeleted)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Item", "Item not found!");
            }
            else
            {
                item.Name = dto.Name;
                item.Description = dto.Description;
                item.ImageUrl = dto.ImageUrl;
                item.CatalogId = dto.CatalogId;
                item.LastModifiedBy = userid;
                item.LastModifiedDate = DateTime.UtcNow;

                _item.Update(item);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseCode.Success;
                response.Message = new("Item", "Item updated!");
            }

            return response;
        }
    }
}
