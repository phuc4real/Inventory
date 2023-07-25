using Inventory.Core.Common;
using Inventory.Core.ViewModel;
using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repository.Repositories
{
    public class UserRepository : Repository<AppUserEntity>, IUserRepository
    {
        private readonly UserManager<AppUserEntity> _userManager;
        public UserRepository(AppDbContext context, UserManager<AppUserEntity> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<AppUserEntity> GetById(string id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _userManager.FindByIdAsync(id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<IEnumerable<AppUserEntity>> GetList()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<Permission> CheckRole(string userId)
        {
            Permission result = new();
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                result.IsTeamLeader = await _userManager.IsInRoleAsync(user, InventoryRoles.TeamLeader);
                result.IsAdmin = await _userManager.IsInRoleAsync(user, InventoryRoles.Admin);
                result.IsSuperAdmin = await _userManager.IsInRoleAsync(user, InventoryRoles.SuperAdmin);
            }

            return result;
        }
    }
}
