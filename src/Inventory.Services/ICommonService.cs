using Inventory.Model.Entity;
using Inventory.Service.Common;
using Inventory.Service.DTO.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service
{
    public interface ICommonService
    {
        public Task<List<AppUser>> GetUsersInRoles(string roleName);
        public Task<Dictionary<string, string>> GetUserFullName(List<string> userNames);
        public Task<CommentResponse> GetComment(int recordId, bool isTicketComment = false);
        public Task<StatusIdCollections> GetStatusCollections();
        public Task<CommentResponse> AddNewComment(CreateCommentRequest request);
    }
}
