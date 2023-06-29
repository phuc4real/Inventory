using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<Item> GetById(Guid id);
        Task<IEnumerable<Item>> GetListItem();
    }
}
