using Inventory.Database.DbContext;
using Inventory.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Implement
{
    internal class StatusRepository : BaseRepository<Status>, IStatusRepository
    {
        public StatusRepository(AppDbContext context) : base(context)
        {
        }
    }
}
