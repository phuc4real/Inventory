using AutoMapper;
using Inventory.Core.Enums;
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
    public class CatalogService : ICatalogServices
    {
        private readonly ICatalogRepository _catalog;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CatalogService(ICatalogRepository catalog, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _catalog = catalog;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultResponse<IEnumerable<CatalogDTO>>> GetAll()
        {
            ResultResponse<IEnumerable<CatalogDTO>> response = new();

            var catalogs = await _catalog.GetAsync();

            response.Data = _mapper.Map<IEnumerable<CatalogDTO>>(catalogs);
            response.Status = ResponeStatus.STATUS_SUCCESS;

            return response;
        }

        public async Task<ResultResponse<CatalogDTO>> GetById(int id)
        {
            ResultResponse<CatalogDTO> response = new() { Messages = new List<ResponseMessage>() };

            var result = await _catalog.FindById(id);

            if (result == null)
            {
                response.Status = ResponeStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Catalog", "Catalog not found!"));
            }
            else
            {
                response.Status = ResponeStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<CatalogDTO>(result);
            }

            return response;
        }

        public async Task<ResultResponse<CatalogDTO>> CreateCatalog(string catalogName)
        {
            ResultResponse<CatalogDTO> response = new() { Messages = new List<ResponseMessage>() };
            Catalog catalog = new Catalog() { Name = catalogName };
            var exists = await _catalog.AnyAsync(x => x.Id == catalog.Id);
            if (exists)
            {
                response.Status = ResponeStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Catalog", "Catalog already exists!"));
            }
            else
            {
                await _catalog.AddAsync(catalog);
                await _unitOfWork.SaveAsync();

                response.Status = ResponeStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<CatalogDTO>(catalog);
                response.Messages.Add(new ResponseMessage("Catalog", "Catalog created!"));
            }

            return response;
        }

        public async Task<ResultResponse<CatalogDTO>> UpdateCatalog(int id, string name)
        {
            ResultResponse<CatalogDTO> response = new() { Messages = new List<ResponseMessage>() };

            var catalog = await _catalog.FindById(id);
            if (catalog == null)
            {
                response.Status = ResponeStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Catalog", "Catalog not exists!"));
            }
            else
            {
                catalog.Name = name;
                _catalog.Update(catalog);
                await _unitOfWork.SaveAsync();

                response.Status = ResponeStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<CatalogDTO>(catalog);
            }
            return response;
        }

        public async Task<ResultResponse<CatalogDTO>> DeleteCatalog(int id)
        {
            ResultResponse<CatalogDTO> response = new() { Messages = new List<ResponseMessage>() };

            var catalog = await _catalog.FindById(id);
            if (catalog == null)
            {
                response.Status = ResponeStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Catalog", "Catalog not exists!"));
            }
            else
            {
                catalog.IsDeleted = !catalog.IsDeleted;
                _catalog.Update(catalog);
                await _unitOfWork.SaveAsync();

                response.Status = ResponeStatus.STATUS_SUCCESS;
                response.Messages.Add(new ResponseMessage("Catalog", "Catalog deleted!"));
            }
            return response;
        }

        public async Task<ResultResponse<IEnumerable<CatalogDTO>>> SearchCatalog(string filter)
        {
            ResultResponse<IEnumerable<CatalogDTO>> response = new() { Messages = new List<ResponseMessage>() };

            IEnumerable<Catalog> catalogs;

            if (int.TryParse(filter, out int id))
                catalogs = await _catalog.GetAsync(x => x.Name!.Contains(filter) || x.Id == id);
            else
                catalogs = await _catalog.GetAsync(x => x.Name!.Contains(filter));

            if (catalogs.Any())
            {
                response.Status = ResponeStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<CatalogDTO>>(catalogs);
            }
            else
            {
                response.Status = ResponeStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Catalog", "Not found!"));
            }

            return response;
        }
    }
}
