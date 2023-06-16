using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _item;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public ItemService(
            IItemRepository item,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITokenService tokenService)
        {
            _item = item;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<ResultResponse<ItemDetailDTO>> CreateItem(string jwtToken, ItemEditDTO dto)
        {
            ResultResponse<ItemDetailDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var userid = _tokenService.GetUserId(jwtToken);

            Item item = _mapper.Map<Item>(dto);
            item.CreatedBy = userid;
            item.CreatedDate = DateTime.Now;
            item.LastModifiedBy = userid;
            item.LastModifiedDate = DateTime.Now;

            await _item.AddAsync(item);
            await _unitOfWork.SaveAsync();

            response.Data = _mapper.Map<ItemDetailDTO>(item);
            response.Status = ResponseStatus.STATUS_SUCCESS;
            return response;
        }

        public async Task<ResultResponse<ItemDetailDTO>> DeleteItem(string jwtToken, Guid id)
        {
            ResultResponse<ItemDetailDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var userid = _tokenService.GetUserId(jwtToken);
            var item = await _item.GetById(id);

            if (item == null || item.IsDeleted)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Item", "Item not found!"));
            }
            else
            {
                item.IsDeleted = true;
                item.LastModifiedBy = userid;
                item.LastModifiedDate = DateTime.Now;
                _item.Update(item);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Messages.Add(new ResponseMessage("Item", "Item deleted!"));
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ItemDetailDTO>>> GetAll()
        {
            ResultResponse<IEnumerable<ItemDetailDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var items = await _item.GetAsync();

            if (items.Any())
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<ItemDetailDTO>>(items);
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Item", "There is no record"));
            }

            return response;
        }

        public async Task<ResultResponse<ItemDetailDTO>> GetById(Guid id)
        {
            ResultResponse<ItemDetailDTO> response = new() 
            { Messages = new List<ResponseMessage>() };

            var item = await _item.GetById(id);
            
            if (item == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Item", "Item not found!"));
            }
            else
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<ItemDetailDTO>(item);
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ItemDetailDTO>>> GetItemInUse()
        {
            ResultResponse<IEnumerable<ItemDetailDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var items = await _item.GetInUseItem();

            if (items.Any())
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<ItemDetailDTO>>(items);
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Item", "There is no record"));
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ItemDetailDTO>>> SearchByName(string name)
        {
            ResultResponse<IEnumerable<ItemDetailDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var items = await _item.GetAsync(x => x.Name!.Contains(name));
            if (items.Any())
            {
                response.Status  = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<ItemDetailDTO>>(items);
            }
            else
            {
                response.Status= ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Item", "Item not found!"));
            }
            return response;
        }

        public async Task<ResultResponse<ItemDetailDTO>> UpdateItem(string jwtToken, Guid id, ItemEditDTO dto)
        {
            ResultResponse<ItemDetailDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var userid = _tokenService.GetUserId(jwtToken);
            var item = await _item.GetById(id);

            if (item == null || item.IsDeleted)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Item", "Item not found!"));
            }
            else
            {
                item.Name = dto.Name;
                item.Description = dto.Description;
                item.ImageUrl = dto.ImageUrl;
                item.CatalogId = dto.CatalogId;
                item.LastModifiedBy = userid;
                item.LastModifiedDate = DateTime.Now;

                _item.Update(item);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Messages.Add(new ResponseMessage("Item", "Item updated!"));
            }

            return response;
        }
    }
}
