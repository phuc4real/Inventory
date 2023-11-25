using AutoMapper;
using Inventory.Core.Constants;
using Inventory.Model.Entity;
using Inventory.Repository;
using Inventory.Service.Common;
using Inventory.Service.DTO.Comment;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public CommonService(IRepoWrapper repoWrapper, IMapper mapper, UserManager<AppUser> userManager)
        {
            _repoWrapper = repoWrapper;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<CommentResponse> AddNewComment(CreateCommentRequest request)
        {
            var response = new CommentResponse();

            var comment = _mapper.Map<Comment>(request);
            comment.CommentAt = DateTime.UtcNow;
            comment.CommentBy = request.GetUserContext();

            await _repoWrapper.Comment.AddAsync(comment);
            await _repoWrapper.SaveAsync();

            response = _mapper.Map<CommentResponse>(comment);
            return response;
        }

        public async Task<Dictionary<string, string>> GetUserFullName(List<string> userNames)
        {
            var users = await _repoWrapper.User.Where(x => userNames.Contains(x.UserName))
                                               .Select(x => new { x.UserName, FullName = x.FirstName + " " + x.LastName })
                                               .ToDictionaryAsync(x => x.UserName!, x => x.FullName);

            return users;
        }

        public async Task<CommentResponse> GetComment(int recordId, bool isTicketComment = false)
        {
            var result = await (
                from comment in _repoWrapper.Comment.FindByCondition(x => x.RecordId == recordId
                                                                                && x.IsTicketComment == isTicketComment)
                join user in _repoWrapper.User
                on comment.CommentBy equals user.UserName
                select new CommentResponse
                {
                    Id = comment.Id,
                    CommentAt = comment.CommentAt,
                    Message = comment.Message,
                    IsReject = comment.IsReject,
                    CommentBy = user.FirstName + " " + user.LastName,

                })
                .OrderByDescending(x => x.CommentAt)
                .FirstOrDefaultAsync();

            return result;

        }

        public async Task<StatusIdCollections> GetStatusCollections()
        {
            var collection = new StatusIdCollections();

            var status = await _repoWrapper.Status.FindAll()
                                                  .ToListAsync();

            collection.Data = status;
            collection.ReviewId = status.FirstOrDefault(x => x.Name == StatusConstant.Review).Id;
            collection.PendingId = status.FirstOrDefault(x => x.Name == StatusConstant.Pending).Id;
            collection.ProcessingId = status.FirstOrDefault(x => x.Name == StatusConstant.Processing).Id;
            collection.CancelId = status.FirstOrDefault(x => x.Name == StatusConstant.Cancel).Id;
            collection.RejectId = status.FirstOrDefault(x => x.Name == StatusConstant.Rejected).Id;
            collection.CloseId = status.FirstOrDefault(x => x.Name == StatusConstant.Close).Id;
            collection.DoneId = status.FirstOrDefault(x => x.Name == StatusConstant.Done).Id;

            return collection;
        }

        public async Task<List<AppUser>> GetUsersInRoles(string roleName)
        {
            var result = await _userManager.GetUsersInRoleAsync(roleName);

            return result.Select(
                        x => new AppUser
                        {
                            UserName = x.UserName,
                            Email = x.Email,
                            FirstName = x.FirstName,
                            LastName = x.LastName
                        }).ToList();
        }
    }
}
