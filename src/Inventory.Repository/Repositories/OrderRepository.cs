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
    public class OrderRepository : Repository<OrderEntity>, IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<OrderEntity> GetAll => _context.Orders;

        private IQueryable<OrderEntity> GetAllIncludeUser => GetAll
            .Include(x => x.CreatedByUser)
            .Include(x => x.UpdatedByUser);

        private IQueryable<OrderEntity> GetAllIncludeHistory => GetAllIncludeUser
            .Include(x => x.History)!
                .ThenInclude(h => h.Decision)
                    .ThenInclude(l => l!.ByUser);

        private IQueryable<OrderEntity> GetAllIncludeDetail => GetAllIncludeHistory
            .Include(x => x.History)!
                .ThenInclude(h => h.Details)!
                    .ThenInclude(d => d.Item);

        public async Task<IEnumerable<OrderEntity>> GetList()
        {
            return await GetAll.ToListAsync();
        }

        public async Task<OrderEntity> GetById(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await GetAllIncludeDetail.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<PaginationList<OrderEntity>> GetPagination(PaginationRequest request)
        {
            PaginationList<OrderEntity> pagination = new();

            var query = GetAllIncludeHistory;

            if (request.SearchKeyword != null)
            {
                var searchKeyword = request.SearchKeyword.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchKeyword) ||
                    x.CreatedByUser!.UserName!.Contains(searchKeyword));
            }

            if (request.SortField != null && request.SortField != "undefined")
            {
                string columnName = StringHelper.CapitalizeFirstLetter(request.SortField);

                var isDesc = request.SortDirection == "desc";

                query = query.OrderByField(columnName, !isDesc);
            }

            pagination.TotalRecords = query.Count();
            pagination.TotalPages = pagination.TotalRecords / request.PageSize;

            query = query.Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize);
            pagination.Data = await query.ToListAsync();

            return pagination;
        }

        public async Task<List<ResponseMessage>> GetCount()
        {
            List<ResponseMessage> result = new();

            var last12Month = DateTime.UtcNow.AddMonths(-11);
            last12Month = last12Month.AddDays(1 - last12Month.Day);
            var query = await GetAll
                .Where(x => x.CreatedDate > last12Month)
                .GroupBy(x => new { x.CreatedDate.Month, x.CreatedDate.Year })
                .ToListAsync();

            for (int i = 1; i <= 12; i++)
            {
                var key = $"{last12Month.Month}/{last12Month.Year}";
                result.Add(new(key, "0"));
                last12Month = last12Month.AddMonths(1);
            }

            foreach (var order in query)
            {
                var key = $"{order.Key.Month}/{order.Key.Year}";
                var index = result.FindIndex(0, x => x.Key == key);
                result[index].Value = order.Count().ToString();
            }

            return result;
        }
    }
}
