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
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<Order> GetAllWithProperty => _context.Orders
            .IgnoreQueryFilters()
            .Include(x => x.OrderByUser)
            .Include(x => x.Details)!
            .ThenInclude(x => x.Item);

        public async Task<IEnumerable<Order>> GetList()
        {
            return await GetAllWithProperty.ToListAsync();
        }

        public async Task<Order> GetById(int id)
        {
            var query = GetAllWithProperty
                .Where(x=> x.Id == id);

#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<PaginationList<Order>> GetPagination(PaginationRequest request)
        {
            PaginationList<Order> pagination = new();

            var query = GetAllWithProperty;

            if(request.SearchKeyword != null)
            {
                var searchKeyword = request.SearchKeyword.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchKeyword) ||
                    x.Items!.Any(i => i.Name!.ToLower().Contains(searchKeyword)) ||
                    x.Items!.Any(i => i.Id!.ToString().ToLower().Contains(searchKeyword))
                    );
            }

            if (request.SortField != null && request.SortField != "undefined")
            {
                string columnName = StringHelper.CapitalizeFirstLetter(request.SortField);

                var desc = request.SortDirection == "desc";

                query = query.OrderByField(columnName, !desc);
            }

            pagination.TotalRecords = query.Count();
            pagination.TotalPages = pagination.TotalRecords / request.PageSize;

            query = query.Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize);
            pagination.Data = await query.ToListAsync();

            return pagination;
        }
    }
}
