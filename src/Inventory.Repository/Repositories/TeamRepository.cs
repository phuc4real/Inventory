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
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Repositories
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        private readonly AppDbContext _context;

        public TeamRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<Team> GetAllWithProperty => _context.Teams
                .Include(x => x.Leader)
                .Include(x => x.Members);

        public async Task<IEnumerable<Team>> GetList()
        {
            return await GetAllWithProperty.ToListAsync();
        }

        public async Task<Team> GetById(Guid id)
        {
            var query = GetAllWithProperty
                .Where(x=>x.Id ==id);

#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<PaginationList<Team>> GetPagination(PaginationRequest request)
        {
            PaginationList<Team> pagination = new();

            var query = GetAllWithProperty;

            if (request.SearchKeyword != null)
            {
                var searchKeyword = request.SearchKeyword.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchKeyword) ||
                    x.Name!.ToLower().Contains(searchKeyword) ||
                    x.Members!.Any(m=>m.UserName!.ToLower().Contains(searchKeyword))
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
