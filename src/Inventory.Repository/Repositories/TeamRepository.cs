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
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        private readonly AppDbContext _context;

        public TeamRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private IQueryable<Team> GetAllWithProperty => _context.Teams
                .Include(x => x.Leader)
                .Include(x => x.Members);

        public async Task<IEnumerable<Team>> GetAllWithPropertyAsync()
        {
            return await GetAllWithProperty.ToListAsync();
        }

        public async Task<Team> GetTeamById(Guid id)
        {
            var query = GetAllWithProperty
                .Where(x=>x.Id ==id);

#pragma warning disable CS8603 // Possible null reference return.
            return await query.FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
