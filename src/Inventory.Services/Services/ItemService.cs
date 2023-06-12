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

        public ItemService(IItemRepository item, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _item = item;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultResponse<ItemDTO>> CreateItem(ItemEditDTO dto)
        {
            ResultResponse<ItemDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            Item item = _mapper.Map<Item>(dto);

            await _item.AddAsync(item);
            await _unitOfWork.SaveAsync();

            response.Data = _mapper.Map<ItemDTO>(item);
            response.Status = ResponseStatus.STATUS_SUCCESS;
            return response;
        }

        public async Task<ResultResponse<ItemDTO>> DeleteItem(Guid id)
        {
            ResultResponse<ItemDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var item = await _item.GetById(id);

            if (item == null || item.IsDeleted)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Item", "Item not found!"));
            }
            else
            {
                item.IsDeleted = true;
                _item.Update(item);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Messages.Add(new ResponseMessage("Item", "Item deleted!"));
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ItemDTO>>> GetAll()
        {
            ResultResponse<IEnumerable<ItemDTO>> response = new();

            var items = await _item.GetAsync();

            response.Status = ResponseStatus.STATUS_SUCCESS;
            response.Data = _mapper.Map<IEnumerable<ItemDTO>>(items);
            return response;
        }

        public async Task<ResultResponse<ItemDTO>> GetById(Guid id)
        {
            ResultResponse<ItemDTO> response = new() 
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
                response.Data = _mapper.Map<ItemDTO>(item);
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ItemDTO>>> SearchByName(string name)
        {
            ResultResponse<IEnumerable<ItemDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var items = await _item.GetAsync(x => x.Name!.Contains(name));
            if (items.Any())
            {
                response.Status  = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<ItemDTO>>(items);
            }
            else
            {
                response.Status= ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Item", "Item not found!"));
            }
            return response;
        }

        public async Task<ResultResponse<ItemDTO>> UpdateItem(Guid id, ItemEditDTO dto)
        {
            ResultResponse<ItemDTO> response = new()
            { Messages = new List<ResponseMessage>() };

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

                _item.Update(item);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Messages.Add(new ResponseMessage("Item", "Item updated!"));
            }

            return response;
        }
    }
}
