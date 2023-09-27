using AutoMapper;
using Inventory.Model.Entity;
using Inventory.Core.Enums;
using Inventory.Repository;
using Inventory.Service.DTO.Category;
using Microsoft.EntityFrameworkCore;
using Inventory.Service.Common;
using Inventory.Core.Common;

namespace Inventory.Service.Implement
{
    public class CategoryServices : ICategoryServices
    {
        #region Ctor & Field

        private readonly IRepoWrapper _repoWrapper;
        private readonly IMapper _mapper;

        public CategoryServices(IRepoWrapper repoWrapper, IMapper mapper)
        {
            _repoWrapper = repoWrapper;
            _mapper = mapper;
        }

        #endregion

        #region Method

        public async Task<CategoryListResponse> GetListAsync()
        {
            CategoryListResponse response = new();

            var categories = await _repoWrapper.Category.FindAll()
                                                       .ToListAsync();

            if (categories.Any())
            {
                response.Data = _mapper.Map<List<CategoryResponse>>(categories);
                response.StatusCode = ResponseCode.Success;
            }
            else
                response.StatusCode = ResponseCode.NoContent;

            return response;
        }

        public async Task<CategoryObjectResponse> GetByIdAsync(int id)
        {
            CategoryObjectResponse response = new();

            var category = await _repoWrapper.Category.FindByCondition(x => x.Id == id)
                                                    .FirstOrDefaultAsync();

            if (category == null)
            {
                response.StatusCode = ResponseCode.NotFound;
                response.Message = new("Catalog", "Not found!");
            }
            else
            {
                response.StatusCode = ResponseCode.Success;
                response.Data = _mapper.Map<CategoryResponse>(category);
            }

            return response;
        }

        public async Task<CategoryObjectResponse> CreateAsync(CategoryUpdateRequest request)
        {
            CategoryObjectResponse response = new();
            Category cate = _mapper.Map<Category>(request);

            await _repoWrapper.Category.
            await _unitOfWork.SaveAsync();

            response.StatusCode = ResponseCode.Created;
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
                response.StatusCode = ResponseCode.NotFound;
                response.Message = new("Catalog", "Catalog not exists!");
            }
            else
            {
                catalog.Name = dto.Name;
                _catalog.Update(catalog);
                await _unitOfWork.SaveAsync();

                response.StatusCode = ResponseCode.Success;
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
                response.StatusCode = ResponseCode.NotFound;
                response.Message = new("Catalog", "Catalog not exists!");
            }
            else
            {
                catalog.IsDeleted = true;
                _catalog.Update(catalog);
                await _unitOfWork.SaveAsync();

                response.StatusCode = ResponseCode.Success;
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
                response.StatusCode = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<Catalog>>(catalogs.Data);
            }
            else
                response.StatusCode = ResponseCode.NoContent;

            return response;
        }

        public async Task<ResultResponse> Any(int id)
        {
            ResultResponse response = new();

            if (await _catalog.AnyAsync(x => x.Id == id))
            {
                response.StatusCode = ResponseCode.Success;
                response.Message = new("Catalog", $"Catalog #{id} exist!");
            }
            else
            {
                response.StatusCode = ResponseCode.NotFound;
                response.Message = new("Catalog", $"Catalog #{id} not exist!");
            }

            return response;
        }

        #endregion
    }
}
