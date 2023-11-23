using AutoMapper;
using Inventory.Model.Entity;
using Inventory.Service.DTO.Comment;
using Inventory.Service.DTO.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.MappingProfile
{
    public class CommentMapping : Profile
    {
        public CommentMapping()
        {
            CreateMap<CreateCommentRequest, Comment>();

            CreateMap<Comment, CommentResponse>();

        }
    }
}
