using AutoMapper;
using Inventory.Model.Entity;
using Inventory.Core.Enums;
using Inventory.Repository;
using Inventory.Service.DTO.Category;
using Microsoft.EntityFrameworkCore;
using Inventory.Service.Common;
using Inventory.Core.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Inventory.Service.Implement
{
    public class CategoryService : ICategoryService
    {
        #region Ctor & Field

        private readonly IRepoWrapper _repoWrapper;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _cacheService;

        public CategoryService(IRepoWrapper repoWrapper, IMapper mapper, IRedisCacheService cacheService)
        {
            _repoWrapper = repoWrapper;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        #endregion

        #region Method

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

            response.Data = _mapper.Map<CategoryResponse>(category);

            return response;
        }

        public async Task<CategoryObjectResponse> CreateAsync(CategoryUpdateRequest request)
        {
            CategoryObjectResponse response = new();

            _repoWrapper.SetUserContext(request.GetUserContext());

            Category cate = _mapper.Map<Category>(request);

            await _repoWrapper.Category.AddAsync(cate);
            await _repoWrapper.SaveAsync();

            response.Data = _mapper.Map<CategoryResponse>(cate);

            return response;
        }

        public async Task<CategoryObjectResponse> UpdateAsync(int id, CategoryUpdateRequest request)
        {
            CategoryObjectResponse response = new();

            var category = await _repoWrapper.Category.FindByCondition(x => x.Id == id)
                                                    .FirstOrDefaultAsync();

            if (category == null || category.IsInactive)
            {
                response.StatusCode = ResponseCode.NotFound;
                response.Message = new("Catalog", "Catalog not exists!");
            }
            else
            {
                category.Name = request.Name;

                _repoWrapper.Category.Update(category);
                await _repoWrapper.SaveAsync();

                response.Data = _mapper.Map<CategoryResponse>(category);

            }
            return response;
        }

        public async Task<BaseResponse> DeactiveAsync(int id)
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
