using Inventory.Core.Extensions;
using Inventory.Core.Helper;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Repositories
{
    public class ExportRepository : Repository<Export>, IExportRepository
    {
        private readonly AppDbContext _context;

        public ExportRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<Export> GetAllWithProperty => _context.Exports
            .Where(x=>!x.IsCancel)
            .Include(x => x.Details)!
                .ThenInclude(x => x.Item)
            .Include(x => x.CreatedByUser); 


        public async Task<IEnumerable<Export>> GetList()
        {
            return await GetAllWithProperty.ToListAsync();
        }

        public async Task<Export> GetById(int id)
        {
            var query = GetAllWithProperty
                .Where(x => x.Id == id);

#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<PaginationList<Export>> GetPagination(PaginationRequest request)
        {
            PaginationList<Export> pagination = new();

            var query = GetAllWithProperty;

            if (request.SearchKeyword != null)
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

        public async Task<List<ResponseMessage>> GetCount()
        {
            List<ResponseMessage> result = new();

            var last12Month = DateTime.UtcNow.AddMonths(-11);
            last12Month = last12Month.AddDays(1 - last12Month.Day);
            var query = await _context.Exports
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
