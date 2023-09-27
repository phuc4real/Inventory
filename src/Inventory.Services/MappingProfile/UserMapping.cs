using AutoMapper;
using Inventory.Model.Entity;
using Inventory.Service.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.MappingProfile
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<AppUser, UserResponse>();
        }
    }
}
