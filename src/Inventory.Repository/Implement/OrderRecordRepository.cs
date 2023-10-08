using Inventory.Database.DbContext;
using Inventory.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Implement
{
    internal class OrderRecordRepository : BaseRepository<OrderRecord>, IOrderRecordRepository
    {
        public OrderRecordRepository(AppDbContext context) : base(context)
        {
        }
    }
}
