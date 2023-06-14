using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;

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
            ResultResponse<IEnumerable<CatalogDTO>> response = new()
            { Messages = new List<ResponseMessage>()};

            var catalogs = await _catalog.GetAsync();

            if(catalogs.Any())
            {
                response.Data = _mapper.Map<IEnumerable<CatalogDTO>>(catalogs);
                response.Status = ResponseStatus.STATUS_SUCCESS;
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Catalog", "There is no record"));
            }

            return response;
        }

        public async Task<ResultResponse<CatalogDTO>> GetById(int id)
        {
            ResultResponse<CatalogDTO> response = new() { Messages = new List<ResponseMessage>() };

            var result = await _catalog.FindById(id);

            if (result == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Catalog", "Catalog not found!"));
            }
            else
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<CatalogDTO>(result);
            }

            return response;
        }

        public async Task<ResultResponse<CatalogDTO>> CreateCatalog(CatalogEditDTO dto)
        {
            ResultResponse<CatalogDTO> response = new() { Messages = new List<ResponseMessage>() };
            Catalog catalog = _mapper.Map<Catalog>(dto);

            await _catalog.AddAsync(catalog);
            await _unitOfWork.SaveAsync();

            response.Status = ResponseStatus.STATUS_SUCCESS;
            response.Data = _mapper.Map<CatalogDTO>(catalog);
            response.Messages.Add(new ResponseMessage("Catalog", "Catalog created!"));

            return response;
        }

        public async Task<ResultResponse<CatalogDTO>> UpdateCatalog(int id, CatalogEditDTO dto)
        {
            ResultResponse<CatalogDTO> response = new() { Messages = new List<ResponseMessage>() };

            var catalog = await _catalog.FindById(id);
            if (catalog == null || catalog.IsDeleted)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Catalog", "Catalog not exists!"));
            }
            else
            {
                catalog.Name = dto.Name;
                _catalog.Update(catalog);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<CatalogDTO>(catalog);
            }
            return response;
        }

        public async Task<ResultResponse<CatalogDTO>> DeleteCatalog(int id)
        {
            ResultResponse<CatalogDTO> response = new() { Messages = new List<ResponseMessage>() };

            var catalog = await _catalog.FindById(id);
            if (catalog == null || catalog.IsDeleted)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Catalog", "Catalog not exists!"));
            }
            else
            {
                catalog.IsDeleted = true;
                _catalog.Update(catalog);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseStatus.STATUS_SUCCESS;
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
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<CatalogDTO>>(catalogs);
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Catalog", "Not found!"));
            }

            return response;
        }
    }
}
