using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IReceiptRepository : IRepository<Receipt>
    {
        Task<PaginationList<Receipt>> GetPagination(PaginationRequest request);
        Task<IEnumerable<Receipt>> GetList();
        Task<Receipt> GetById(int id);
        Task<List<ResponseMessage>> GetCount();
    }
}
