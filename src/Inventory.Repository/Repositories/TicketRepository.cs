using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Core.Helper;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repository.Repositories
{
    public class TicketRepository : Repository<TicketEntity>, ITicketRepository
    {
        private readonly AppDbContext _context;
        public TicketRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<TicketEntity> GetAll => _context.Tickets;

        private IQueryable<TicketEntity> GetAllIncludeUser => GetAll
            .Include(x => x.CreatedByUser)
            .Include(x => x.UpdatedByUser);

        private IQueryable<TicketEntity> GetAllIncludeHistory => GetAllIncludeUser
            .Include(x => x.History)!
                .ThenInclude(h => h.LeaderDecision)
                    .ThenInclude(l => l!.ByUser)
            .Include(x => x.History)!
                .ThenInclude(h => h.Decision)
                    .ThenInclude(l => l!.ByUser);

        private IQueryable<TicketEntity> GetAllIncludeDetail => GetAllIncludeHistory
            .Include(x => x.History)!
                .ThenInclude(h => h.Details)!
                    .ThenInclude(d => d.Item);

        public async Task<TicketEntity> GetById(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await GetAllIncludeDetail.FirstOrDefaultAsync(x => x.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<PaginationList<TicketEntity>> GetPagination(PaginationRequest request)
        {
            return await Pagination(request, GetAllIncludeDetail);
        }


        public async Task<PaginationList<TicketEntity>> GetPagination(PaginationRequest request, Guid teamId)
        {
            return await Pagination(request, GetAllIncludeDetail.Where(x => x.CreatedByUser!.TeamId == teamId));
        }

        public async Task<PaginationList<TicketEntity>> GetPagination(PaginationRequest request, string userId)
        {
            return await Pagination(request, GetAllIncludeDetail.Where(x => x.CreatedById == userId));
        }

        private static async Task<PaginationList<TicketEntity>> Pagination(PaginationRequest request, IQueryable<TicketEntity> query)
        {
            PaginationList<TicketEntity> pagination = new();

            if (request.SearchKeyword != null)
            {
                var searchKeyword = request.SearchKeyword.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchKeyword) ||
                    x.CreatedByUser!.UserName!.Contains(searchKeyword)
                    );
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

        public async Task<IEnumerable<TicketEntity>> GetList()
        {
            return await GetAll.ToListAsync();
        }

        public async Task<TicketCount> GetCount()
        {
            TicketCount result = new()
            {
                Pending = 0,
                Processing = 0,
                Completed = 0,
                Rejected = 0,
            };

            var month = DateTime.Now.Month;

            var query = GetAllIncludeHistory.Where(x => x.CreatedDate.Month == month)
                .Select(x => new TicketEntity
                {
                    Id = x.Id,
                    History = x.History!
                    .OrderByDescending(h => h.CreatedAt)
                    .Take(1)
                    .ToList(),
                });

            var groupBy = await query
                .Select(x => new { x.Id, x.History!.First().Status })
                .GroupBy(x => x.Status)
                .ToListAsync();

            foreach (var item in groupBy)
            {
                if (item.Key == TicketStatus.Pending) result.Pending = item.Count();
                if (item.Key == TicketStatus.Processing) result.Processing = item.Count();
                if (item.Key == TicketStatus.Done) result.Completed = item.Count();
                if (item.Key == TicketStatus.Reject) result.Rejected = item.Count();
            }

            return result;
        }
    }
}
