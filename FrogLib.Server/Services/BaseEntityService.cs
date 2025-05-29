using FrogLib.Server.DTOs;
using FrogLib.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace FrogLib.Server.Services
{
    public abstract class BaseEntityService(FroglibContext context, string typeObject)
    {
        private readonly FroglibContext _context = context;
        private readonly string _typeObject = typeObject;

        public async Task<double> GetRatingAsync(int idEntity)
        {
            var totalRatings = await _context.Entityratings
                .CountAsync(er => er.TypeEntity == _typeObject && er.IdEntity == idEntity);
            if (totalRatings == 0) { return 0; }

            var positiveRatings = await _context.Entityratings
                .CountAsync(er => er.TypeEntity == _typeObject && er.IdEntity == idEntity && er.Rating == 1);

            return (positiveRatings * 100.0) / totalRatings;
        }

        public async Task<int> GetCountViewAsync(int idEntity)
        {
            return await _context.Viewhistories
                .CountAsync(vh => vh.TypeEntity == _typeObject && vh.IdEntity == idEntity);
        }

        public async Task<int> GetCountCommentsAsync(int idEntity)
        {
            return await _context.Comments
                .CountAsync(cm => cm.TypeEntity == _typeObject && cm.IdEntity == idEntity);
        }

        public async Task<List<CommentDTO>> GetCommentsAsync(int idEntity)
        {
            var allComments = await _context.Comments
                .Where(c => c.TypeEntity == _typeObject && c.IdEntity == idEntity)
                .Include(c => c.IdUserNavigation)
                .ToListAsync();

            var comments = allComments
                .Where(c => c.ParentCommentId == null)
                .Select(c => MapComment(c, allComments))
                .ToList();

            return comments;
        }

        private CommentDTO MapComment(Comment comment, List<Comment> allComments)
        {
            return new CommentDTO
            {
                Id = comment.IdComment,
                Author = comment.IdUserNavigation.NameUser,
                AuthorURL = comment.IdUserNavigation.ProfileImageUrl,
                Date = comment.DateComment,
                Content = comment.TextComment,
                Status = comment.StatusComment,
                IsReply = comment.ParentCommentId != null,
                Replies = allComments
                    .Where(r => r.ParentCommentId == comment.IdComment)
                    .Select(r => MapComment(r, allComments))
                    .ToList()
            };
        }
    }
}
