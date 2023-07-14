using Inventory.Core.Extensions;
using Inventory.Core.Helper;
using Inventory.Core.Request;
using Inventory.Core.ViewModel;
using Inventory.Repository.DbContext;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repository.Repositories
{
    public class UserRepository : Repository<AppUser>, IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        public UserRepository(AppDbContext context, UserManager<AppUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<PaginationList<AppUser>> GetPagination(PaginationRequest request)
        {
            PaginationList<AppUser> pagination = new();

            var list = _userManager.Users.AsQueryable();

            if (request.SearchKeyword != null)
            {
                var searchKeyword = request.SearchKeyword.ToLower();
                list = list.Where(x =>
                    x.UserName!.ToLower().Contains(searchKeyword) ||
                    x.Email!.ToLower().Contains(searchKeyword) ||
                    string.Concat(x.FirstName, x.LastName).Contains(searchKeyword)
                );
            }

            pagination.TotalRecords = list.Count();
            pagination.TotalPages = pagination.TotalRecords /request.PageSize;

            if(request.SortField != null && request.SortField != "undefined")
            {
                string columnName = StringHelper.CapitalizeFirstLetter(request.SortField);
                var desc = request.SortDirection == "desc";
                list = list.OrderByField(columnName, !desc);
            }

            list = list.Skip(request.PageIndex * request.PageSize)
                         .Take(request.PageSize);

            pagination.Data = await list.ToListAsync();

            return pagination;
        }

        public async Task<AppUser> GetById(string id)
        {
            var result = await _userManager.FindByIdAsync(id);

#pragma warning disable CS8603 // Possible null reference return.
            return result;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<IEnumerable<AppUser>> GetList()
        {
            var result = await _userManager.Users.ToListAsync();

            return result;
        }
    }
}
