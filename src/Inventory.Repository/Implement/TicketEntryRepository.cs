using Inventory.Database.DbContext;
using Inventory.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Implement
{
    internal class TicketEntryRepository : BaseRepository<TicketEntry>, ITicketEntryRepository
    {
        public TicketEntryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
