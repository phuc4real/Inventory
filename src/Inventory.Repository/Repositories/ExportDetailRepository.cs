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

        private IQueryable<ExportDetail> GetAllWithProperty => _context.ExportDetails
            .Include(x => x.Item)
            .Include(x => x.Export)
            .Include(x => x.ForUser);

        public async Task<IEnumerable<ExportDetail>> GetUsingItem()
        {
            return await GetAllWithProperty.ToListAsync();
        }

        public async Task<IEnumerable<ExportDetail>> GetUsingItemByTeam(Guid teamId)
        {
            var query = GetAllWithProperty
                .Where(x => x.ForUser!.TeamId == teamId);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<ExportDetail>> GetUsingItemByUser(string userId)
        {
            var query = GetAllWithProperty
                .Where(x=>x.ForUserId == userId);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<ExportDetail>> SearchAsync(string filter)
        {
            _ = int.TryParse(filter, out int eid);
            _ = Guid.TryParse(filter, out Guid iid);

            var query = GetAllWithProperty
                .Where(x => x.ItemId.Equals(iid) ||
                            x.Item!.Name!.Contains(filter) ||
                            x.ExportId.Equals(eid) ||
                            x.ForUser!.UserName!.Contains(filter) ||
                            x.ForUser.Id.Equals(filter) ||
                            x.ForUser.Email!.Equals(filter)
                );

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<ExportDetail>> SearchInTeamAsync(Guid teamId, string filter)
        {
            _ = int.TryParse(filter, out int eid);
            _ = Guid.TryParse(filter, out Guid iid);

            var query = GetAllWithProperty
                .Where(x => x.ForUser!.TeamId == teamId)
                .Where(x => x.ItemId.Equals(iid) ||
                            x.Item!.Name!.Contains(filter) ||
                            x.ExportId.Equals(eid) ||
                            x.ForUser!.UserName!.Contains(filter) ||
                            x.ForUser.Id.Equals(filter) ||
                            x.ForUser.Email!.Equals(filter)
                );

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<ExportDetail>> SearchMyItemAsync(string userId, string filter)
        {
            _ = int.TryParse(filter, out int eid);
            _ = Guid.TryParse(filter, out Guid iid);

            var query = GetAllWithProperty
                .Where(x => x.ForUserId == userId)
                .Where(x => x.ItemId.Equals(iid) ||
                            x.Item!.Name!.Contains(filter) ||
                            x.ExportId.Equals(eid)
                );

            return await query.ToListAsync();
        }
    }
}
