using AutoMapper;
using Inventory.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Implement
{
    public class CommonService : ICommonService
    {
        private readonly IRepoWrapper _repoWrapper;
        private readonly IMapper _mapper;

        public CommonService(IRepoWrapper repoWrapper, IMapper mapper)
        {
            _repoWrapper = repoWrapper;
            _mapper = mapper;
        }

        public async Task<(string, string)> GetAuditUserData(string createdBy, string updatedBy)
        {
            var users = await _repoWrapper.User.Where(x => x.UserName == createdBy || x.UserName == updatedBy)
                                               .ToListAsync();

            var createdUser = users.Where(x => x.UserName == createdBy).FirstOrDefault();
            var fullNameCreatedUser = createdUser.FirstName + " " + createdUser.LastName;

            var updatedUser = users.Where(x => x.UserName == updatedBy).FirstOrDefault();
            var fullNameUpdatedUser = updatedUser.FirstName + " " + updatedUser.LastName;

            return (fullNameCreatedUser, fullNameUpdatedUser);
        }
    }
}
