using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IReceiptRepository : IRepository<Receipt>
    {
        Task<Receipt> GetById(int id);
        Task<IEnumerable<Receipt>> ReceiptByItem(Item item);
        Task<IEnumerable<Receipt>> GetAllAsync();
    }
}
