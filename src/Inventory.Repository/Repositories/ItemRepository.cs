using Inventory.Core.Request;
using Inventory.Core.Response;
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
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Items.FindAsync(id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<PaginationList<Item>> GetListItem(ListItemRequest requestParams)
        {
            PaginationList<Item> items = new();
            var query = GetAllWithProperty;

            if (requestParams.SearchKeyword != null)
            {
                query = query.Where(x =>
                    x.Name!.Contains(requestParams.SearchKeyword) ||
                    x.Catalog!.Name!.Contains(requestParams.SearchKeyword)
                    );
            }

            items.TotalRecords = query.Count();
            items.TotalPages = items.TotalRecords / requestParams.PageSize;

            query = query.Skip(requestParams.PageIndex * requestParams.PageSize)
                .Take(requestParams.PageSize);
            items.Data = await query.ToListAsync();

            return items;
        }
    }
}
