using AutoMapper;
using Inventory.Model.Entity;
using Inventory.Service.DTO.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.MappingProfile
{
    public class CategoryMapping : Profile
    {
        public CategoryMapping()
        {
            CreateMap<CategoryUpdateRequest, Category>();
            CreateMap<Category, CategoryResponse>();
        }
    }
}
