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
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        private readonly AppDbContext _context;
        public ItemRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<Item> GetAllWithProperty => _context.Items
            .Include(x => x.Catalog)
            .Include(x => x.CreatedByUser)
            .Include(x => x.ModifiedByUser);

        public async Task<Item> GetById(Guid id)
        {
            var query = GetAllWithProperty
                .Where(x => x.Id == id);
#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<PaginationList<Item>> GetListItem(PaginationRequest requestParams)
        {
            PaginationList<Item> items = new();
            var query = GetAllWithProperty;

            if (requestParams.SearchKeyword != null)
            {
                var searchKeyword = requestParams.SearchKeyword.ToLower();
                query = query.Where(x =>
                    x.Id.ToString().ToLower().Contains(searchKeyword) ||
                    x.Name!.ToLower().Contains(searchKeyword) ||
                    x.Description!.ToLower().Contains(searchKeyword) ||
                    x.Catalog!.Name!.ToLower().Contains(searchKeyword)
                    );
            }

            items.TotalRecords = query.Count();
            items.TotalPages = items.TotalRecords / requestParams.PageSize;

            if( requestParams.SortField != null && requestParams.SortField != "undefined")
            {
                string columnName = StringHelper.CapitalizeFirstLetter(requestParams.SortField);

                var desc = requestParams.SortDirection == "desc";

                query = query.OrderByField(columnName, !desc);
            }

            query = query.Skip(requestParams.PageIndex * requestParams.PageSize)
                .Take(requestParams.PageSize);
            items.Data = await query.ToListAsync();

            return items;
        }

        public async Task<IEnumerable<Item>> Search(string name)
        {
            IQueryable<Item> query = _context.Items;

            if (name != null)
            {
                query = query.Where(x => x.Name!.ToLower().Contains(name.ToLower()));
            }

            return await query.Take(10).ToListAsync();
        }
    }
}
