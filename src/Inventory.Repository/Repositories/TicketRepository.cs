﻿using Inventory.Core.Enums;
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
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        private readonly AppDbContext _context;
        public TicketRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<Ticket> GetAllWithProperty => _context.Tickets
            .Include(x => x.Details)!
            .ThenInclude(d => d.Item)
            .Include(x => x.CreatedByUser)
            .Include(x => x.ModifiedByUser);

        public async Task<Ticket> GetById(Guid id)
        {
            var query = GetAllWithProperty
                .Where(x => x.Id == id);

#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<PaginationList<Ticket>> GetPagination(PaginationRequest request)
        {
            PaginationList<Ticket> pagination = new();

            var query = GetAllWithProperty;

            if (request.SearchKeyword != null)
            {
                var searchKeyword = request.SearchKeyword.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().Contains(searchKeyword) ||
                    x.Title!.Contains(searchKeyword) ||
                    x.Description!.Contains(searchKeyword) ||
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

        public async Task<IEnumerable<Ticket>> GetList(Guid teamId)
        {
            var query = GetAllWithProperty
                .Where(x => x.CreatedByUser!.TeamId == teamId);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetList(string userid)
        {
            var query = GetAllWithProperty
                .Where(x => x.CreatedBy == userid);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetList(Item item)
        {
            var query = GetAllWithProperty
                .Where(x => x.Items!.Contains(item));

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetList()
        {
            var query = GetAllWithProperty;

            return await query.ToListAsync();
        }

        public async Task<TicketCountDTO> GetCount()
        {
            TicketCountDTO result = new()
            {
                Pending = 0,
                Processing = 0,
                Completed = 0,
                Rejected = 0,
            };

            var month = DateTime.Now.Month;

            var query = _context.Tickets.Where(x=> x.CreatedDate.Month == month);

            var groupBy = await query.Select(x => new { x.Id, x.Status }).GroupBy(x => x.Status).ToListAsync() ;

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
