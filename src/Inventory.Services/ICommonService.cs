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
        public Task<(string, string)> GetAuditUserData(string createdBy, string updatedBy);
        public Task<CommentResponse> GetComment(int recordId, bool isTicketComment = false);
        public Task<StatusIdCollections> GetStatusCollections();
        public Task<CommentResponse> AddNewComment(CreateCommentRequest request);
    }
}
