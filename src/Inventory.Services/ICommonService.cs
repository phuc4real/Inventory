using Inventory.Model.Entity;
using Inventory.Service.Common;
using Inventory.Service.DTO.Comment;
using Inventory.Service.DTO.User;

namespace Inventory.Service
{
    public interface ICommonService
    {
        public Task<CommentResponse> GetComment(int recordId, bool isTicketComment = false);
        public Task<StatusIdCollections> GetStatusCollections();
        public Task<CommentResponse> AddNewComment(CreateCommentRequest request);
    }
}
