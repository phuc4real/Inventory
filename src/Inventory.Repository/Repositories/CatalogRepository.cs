using Inventory.Core.Extensions;
using Inventory.Core.Helper;
using Inventory.Core.Request;
using Inventory.Core.ViewModel;
using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repository.Repositories
{
    public class CatalogRepository : Repository<Catalog>, ICatalogRepository
    {
        private readonly AppDbContext _context;

        public CatalogRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<Catalog> GetAll => _context.Catalogs; 

        public async Task<Catalog> GetById(int id)
        {
            var query = GetAll.Where(x => x.Id == id);
#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<PaginationList<Catalog>> GetPagination(PaginationRequest request)
        {
            PaginationList<Catalog> catalogs = new();

            var query = GetAll;

            if (request.SearchKeyword != null)
            {
                var searchKeyword = request.SearchKeyword.ToLower();
                query = query.Where(x => 
                    x.Id.ToString().Contains(searchKeyword) ||
                    x.Name!.ToLower().Contains(searchKeyword)
                 );
            }

            catalogs.TotalRecords = query.Count();
            catalogs.TotalPages = catalogs.TotalRecords / request.PageSize;

            if (request.SortField != null && request.SortField != "undefined")
            {
                string columnName = StringHelper.CapitalizeFirstLetter(request.SortField);

                var desc = request.SortDirection == "desc";

                query = query.OrderByField(columnName, !desc);
            }

            query = query.Skip(request.PageIndex * request.PageSize)
                         .Take(request.PageSize);

            catalogs.Data = await query.ToListAsync();

            return catalogs;
        }
        public async Task<IEnumerable<Catalog>> GetList(string name)
        {
            IQueryable<Catalog> query = _context.Catalogs;

            if (name != null)
            {
                query = query.Where(x => x.Name!.ToLower().Contains(name.ToLower()));
            }

            return await query.Take(10).ToListAsync();
        }
    }
}
