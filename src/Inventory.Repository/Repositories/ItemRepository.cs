using Inventory.Core.Extensions;
using Inventory.Core.Helper;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repository.Repositories
{
    public class ItemRepository : Repository<ItemEntity>, IItemRepository
    {
        private readonly AppDbContext _context;
        public ItemRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<ItemEntity> GetAll => _context.Items;

        private IQueryable<ItemEntity> GetAllIncludeProperty => GetAll
             .Include(x => x.Catalog)
            .Include(x => x.CreatedByUser)
            .Include(x => x.UpdatedByUser);

        public async Task<ItemEntity> GetById(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await GetAllIncludeProperty.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<PaginationList<ItemEntity>> GetPagination(PaginationRequest requestParams)
        {
            PaginationList<ItemEntity> items = new();
            var query = GetAllIncludeProperty.Where(x => !x.IsDeleted);

            if (requestParams.SearchKeyword != null)
            {
                var searchKeyword = requestParams.SearchKeyword.ToLower();
                query = query.Where(x =>
                    x.Code!.ToLower().Contains(searchKeyword) ||
                    x.Name!.ToLower().Contains(searchKeyword) ||
                    x.Catalog!.Name!.ToLower().Contains(searchKeyword)
                    );
            }

            items.TotalRecords = query.Count();
            items.TotalPages = items.TotalRecords / requestParams.PageSize;

            if (requestParams.SortField != null && requestParams.SortField != "undefined")
            {
                string columnName = StringHelper.CapitalizeFirstLetter(requestParams.SortField);

                var isDesc = requestParams.SortDirection == "desc";

                query = query.OrderByField(columnName, !isDesc);
            }

            query = query.Skip(requestParams.PageIndex * requestParams.PageSize)
                .Take(requestParams.PageSize);
            items.Data = await query.ToListAsync();

            return items;
        }

        public async Task<IEnumerable<ItemEntity>> GetList(string filter)
        {
            var query = GetAll.Where(x => !x.IsDeleted);

            if (filter != null)
            {
                if (Guid.TryParse(filter, out var id))
                {
                    query = query.Where(x => x.Id == id);
                }
                else
                {
                    query = query.Where(x => x.Name!.ToLower().Contains(filter.ToLower()));
                }
            }

            return await query.Take(10).ToListAsync();
        }

        public async Task<IEnumerable<ItemEntity>> GetRange(List<Guid> ids)
        {
            return await GetAll
                    .Where(x => ids.Contains(x.Id))
                    .ToListAsync();
        }
    }
}
