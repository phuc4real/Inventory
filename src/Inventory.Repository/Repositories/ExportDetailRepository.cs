using Inventory.Core.Extensions;
using Inventory.Core.Helper;
using Inventory.Core.Request;
using Inventory.Core.ViewModel;
using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Repositories
{
    public class ExportDetailRepository : Repository<ExportDetail>, IExportDetailRepository
    {
        private readonly AppDbContext _context;
        public ExportDetailRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<ExportDetail> GetAllWithProperty => _context.ExportDetails
            .Include(x => x.Item)
            .Include(x => x.Export)
            .Include(x => x.ForUser);

        public async Task<IEnumerable<ExportDetail>> GetList()
        {
            var query = GetAllWithProperty;

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<ExportDetail>> GetList(Guid teamId)
        {
            var query = GetAllWithProperty
                .Where(x => x.ForUser!.TeamId == teamId);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<ExportDetail>> GetList(string userId)
        {
            var query = GetAllWithProperty
                .Where(x=>x.ForUserId == userId);

            return await query.ToListAsync();
        }

        public async Task<PaginationList<ExportDetail>> GetPagination(PaginationRequest request)
        {
            PaginationList<ExportDetail> pagination = new();

            var query = GetAllWithProperty;

            if (request.SearchKeyword != null)
            {
                var searchKeyword = request.SearchKeyword.ToLower();
                query = query.Where(x =>
                    x.ItemId.ToString().Contains(searchKeyword) ||
                    x.Item!.Name!.Contains(searchKeyword) ||
                    x.ExportId.Equals(searchKeyword) ||
                    x.ForUser!.UserName!.Contains(searchKeyword) ||
                    x.ForUser.Id.Equals(searchKeyword) ||
                    x.ForUser.Email!.Equals(searchKeyword)
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
