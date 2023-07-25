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
    public class ExportDetailRepository : Repository<ExportDetailEntity>, IExportDetailRepository
    {
        private readonly AppDbContext _context;
        public ExportDetailRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<PaginationList<ExportDetailEntity>> GetPagination(PaginationRequest request)
        {
            return Pagination(request, GetAllInclude);
        }

        public Task<PaginationList<ExportDetailEntity>> GetPagination(PaginationRequest request, string userId)
        {
            return Pagination(request, GetAllInclude.Where(x => x.Export!.ForId == userId));
        }

        public Task<PaginationList<ExportDetailEntity>> GetPagination(PaginationRequest request, Guid teamId)
        {
            return Pagination(request, GetAllInclude.Where(x => x.Export!.ForUser!.TeamId == teamId));
        }

        private IQueryable<ExportDetailEntity> GetAll => _context.ExportDetails;

        private IQueryable<ExportDetailEntity> GetAllInclude => GetAll
            .Include(x => x.Item)
            .Include(x => x.Export)
                .ThenInclude(e => e!.ForUser);


        private static async Task<PaginationList<ExportDetailEntity>> Pagination(PaginationRequest request, IQueryable<ExportDetailEntity> query)
        {
            PaginationList<ExportDetailEntity> pagination = new();

            if (request.SearchKeyword != null)
            {
                var searchKeyword = request.SearchKeyword.ToLower();
                query = query.Where(x =>
                    x.ItemId.ToString().Contains(searchKeyword) ||
                    x.Item!.Name!.Contains(searchKeyword) ||
                    x.ExportId.Equals(searchKeyword)
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
    }
}
