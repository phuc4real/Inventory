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

        public async Task<ResultResponse<IEnumerable<Catalog>>> GetList()
        {
            ResultResponse<IEnumerable<Catalog>> response = new();

            var catalogs = await _catalog.GetAsync();

            if (catalogs.Any())
            {
                response.Data = _mapper.Map<IEnumerable<Catalog>>(catalogs);
                response.Status = ResponseCode.Success;
            }
            else
                response.Status = ResponseCode.NoContent;

            return response;
        }

        public async Task<ResultResponse<Catalog>> GetById(int id)
        {
            ResultResponse<Catalog> response = new();

            var result = await _catalog.GetById(id);

            if (result == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Catalog", "Not found!");
            }
            else
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<Catalog>(result);
            }

            return response;
        }

        public async Task<ResultResponse<Catalog>> Create(UpdateCatalog dto)
        {
            ResultResponse<Catalog> response = new();
            CatalogEntity catalog = _mapper.Map<CatalogEntity>(dto);

            await _catalog.AddAsync(catalog);
            await _unitOfWork.SaveAsync();

            response.Status = ResponseCode.Created;
            response.Data = _mapper.Map<Catalog>(catalog);
            response.Message = new("Catalog", "Catalog created!");

            return response;
        }

        public async Task<ResultResponse> Update(int id, UpdateCatalog dto)
        {
            ResultResponse response = new();

            var catalog = await _catalog.GetById(id);
            if (catalog == null || catalog.IsDeleted)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Catalog", "Catalog not exists!");
            }
            else
            {
                catalog.Name = dto.Name;
                _catalog.Update(catalog);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseCode.Success;
                response.Message = new("Catalog", "Catalog updated!");
            }
            return response;
        }

        public async Task<ResultResponse> Delete(int id)
        {
            ResultResponse response = new();

            var catalog = await _catalog.GetById(id);
            if (catalog == null || catalog.IsDeleted)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Catalog", "Catalog not exists!");
            }
            else
            {
                catalog.IsDeleted = true;
                _catalog.Update(catalog);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseCode.Success;
                response.Message = new("Catalog", "Catalog deleted!");
            }
            return response;
        }

        public async Task<PaginationResponse<Catalog>> GetPagination(PaginationRequest request)
        {
            PaginationResponse<Catalog> response = new()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            var catalogs = await _catalog.GetPagination(request);

            if (catalogs.Data!.Any())
            {
                response.TotalRecords = catalogs.TotalRecords;
                response.TotalPages = catalogs.TotalPages;
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<Catalog>>(catalogs.Data);
            }
            else
                response.Status = ResponseCode.NoContent;

            return response;
        }

        public async Task<ResultResponse> Any(int id)
        {
            ResultResponse response = new();

            if (await _catalog.AnyAsync(x => x.Id == id))
            {
                response.Status = ResponseCode.Success;
                response.Message = new("Catalog", $"Catalog #{id} exist!");
            }
            else
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Catalog", $"Catalog #{id} not exist!");
            }

            return response;
        }
    }
}
