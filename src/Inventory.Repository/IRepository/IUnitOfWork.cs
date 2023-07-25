namespace Inventory.Repository.IRepository
{
    public interface IUnitOfWork
    {
        Task SaveAsync();
    }
}
