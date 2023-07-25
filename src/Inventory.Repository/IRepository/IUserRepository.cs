﻿using Inventory.Core.ViewModel;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IUserRepository : IRepository<AppUserEntity>
    {
        Task<IEnumerable<AppUserEntity>> GetList();
        Task<AppUserEntity> GetById(string id);
        Task<Permission> CheckRole(string userId);
    }
}
