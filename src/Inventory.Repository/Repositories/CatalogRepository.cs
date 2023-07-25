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
    public class CatalogRepository : Repository<CatalogEntity>, ICatalogRepository
    {
        private readonly AppDbContext _context;

        public CatalogRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<CatalogEntity> GetAll => _context.Catalogs;

        public async Task<CatalogEntity> GetById(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Catalogs.FindAsync(id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<PaginationList<CatalogEntity>> GetPagination(PaginationRequest request)
        {
            PaginationList<CatalogEntity> catalogs = new();

            var query = GetAll.Where(x => !x.IsDeleted);

            if (request.SearchKeyword != null)
            {
                var searchKeyword = request.SearchKeyword.ToLower();
                query = query.Where(x => x.Name!.ToLower().Contains(searchKeyword));
            }

            catalogs.TotalRecords = query.Count();
            catalogs.TotalPages = catalogs.TotalRecords / request.PageSize;

            if (request.SortField != null && request.SortField != "undefined")
            {
                string columnName = StringHelper.CapitalizeFirstLetter(request.SortField);

                var isDesc = request.SortDirection == "desc";

                query = query.OrderByField(columnName, !isDesc);
            }

            query = query.Skip(request.PageIndex * request.PageSize)
                         .Take(request.PageSize);

            catalogs.Data = await query.ToListAsync();

            return catalogs;
        }
        public async Task<IEnumerable<CatalogEntity>> GetList(string name)
        {
            var query = _context.Catalogs.Where(x => !x.IsDeleted);

            if (name != null)
            {
                query = query.Where(x => x.Name!.ToLower().Contains(name.ToLower()));
            }

            return await query.Take(10).ToListAsync();
        }
    }
}
