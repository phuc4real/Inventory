using Inventory.Core.Request;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.IRepository
{
    public interface ITeamRepository : IRepository<Team>
    {
        Task<PaginationList<Team>> GetPagination(PaginationRequest request);
        Task<IEnumerable<Team>> GetList();
        Task<Team> GetById(Guid id);
    }
}
