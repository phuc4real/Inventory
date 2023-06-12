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

        public async Task<IEnumerable<Team>> GetAllWithPropertyAsync()
        {
            var teams = _context.Teams.Include(x => x.Leader);

            return await teams.ToListAsync();
        }

        public async Task<Team> GetTeamById(Guid id)
        {
#pragma warning disable CS8603 // Possible null reference return.

            var team = await _context.Teams.Where(x=> x.Id == id).Include(x=> x.Leader).FirstOrDefaultAsync();
            return team;
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
