using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Repositories
{
    public class ExportDetailRepository : Repository<ExportDetail>, IExportDetailRepository
    {
        private readonly AppDbContext _context;
        public ExportDetailRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExportDetail>> GetAllAsync()
        {
            IQueryable<ExportDetail> query = _context.ExportDetails;
            query = query.Include(x => x.Item)
                .Include(x => x.Export)
                .Include(x => x.ForUser);

            return await query.ToListAsync();
        }

        //Search by Itemid, item name ,exportid ,username, userid, email
        public async Task<IEnumerable<ExportDetail>> SearchAsync(string filter)
        {
            IQueryable<ExportDetail> query = _context.ExportDetails;
            query = query.Include(x => x.Item)
                .Include(x => x.Export)
                .Include(x => x.ForUser);

            _ = int.TryParse(filter, out int eid);

            _ = Guid.TryParse(filter, out Guid iid);

            query = query.Where(x => 
                x.ItemId.Equals(iid) ||
                x.Item!.Name!.Contains(filter) ||
                x.ExportId.Equals(eid) ||
                x.ForUser!.UserName!.Contains(filter) ||
                x.ForUser.Id.Equals(filter) ||
                x.ForUser.Email!.Equals(filter)
            );

            return await query.ToListAsync();
        }
    }
}
