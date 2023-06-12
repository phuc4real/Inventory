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
        
        Task<IEnumerable<Team>> GetAllWithPropertyAsync();
        Task<Team> GetTeamById(Guid id);
    }
}
