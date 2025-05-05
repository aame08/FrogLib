using FrogLib.Server.DTOs;
using FrogLib.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrogLib.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModerController(FroglibContext context) : BaseController
    {
        private readonly FroglibContext _context = context;

        [Authorize(Roles = "Модератор")]
        [HttpGet("moder/reviews")]
        public async Task<ActionResult> GetModerReviewsAsync()
        {
            try
            {
                var reviews = await _context.Reviews
                    .Where(r => r.StatusReview == "На рассмотрении")
                    .Include(r => r.IdBookNavigation)
                    .Include(r => r.IdUserNavigation)
                    .Select(review => new
                    {
                        IdReview = review.IdReview,
                        TitleReview = review.TitleReview,
                        TextReview = review.TextReview,
                        Author = new
                        {
                            Id = review.IdUserNavigation.IdUser,
                            Name = review.IdUserNavigation.NameUser
                        },
                        Book = new
                        {
                            IdBook = review.IdBookNavigation.IdBook,
                            Title = review.IdBookNavigation.TitleBook,
                            ImageURL = review.IdBookNavigation.ImageUrl
                        },
                        UserRating = _context.Bookratings
                        .FirstOrDefault(r => r.IdUser == review.IdUser && r.IdBook == review.IdBook) != null
                            ? _context.Bookratings
                            .FirstOrDefault(r => r.IdUser == review.IdUser && r.IdBook == review.IdBook)
                            .Rating
                            : (int?)null
                    })
                    .ToListAsync();

                return Ok(reviews);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Модератор")]
        [HttpGet("moder/collections")]
        public async Task<ActionResult> GetModerCollectionsAsync()
        {
            try
            {
                var collections = await _context.Collections
                    .Where(c => c.StatusCollection == "На рассмотрении")
                    .Include(c => c.IdUserNavigation)
                    .Include(c => c.IdBooks)
                    .Select(collection => new
                    {
                        IdCollection = collection.IdCollection,
                        TitleCollection = collection.TitleCollection,
                        TextCollection = collection.DescriptionCollection,
                        Author = new
                        {
                            Id = collection.IdUserNavigation.IdUser,
                            Name = collection.IdUserNavigation.NameUser
                        },
                        CountBooks = collection.IdBooks.Count,
                        Books = collection.IdBooks.Select(book => new
                        {
                            IdBook = book.IdBook,
                            Title = book.TitleBook,
                            ImageURL = book.ImageUrl
                        })
                    })
                    .ToListAsync();

                return Ok(collections);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Модератор")]
        [HttpPut("moder/update-review-status/{idReview}/{status}")]
        public async Task<ActionResult> UpdateReviewStatusAsync(int idReview, string status)
        {
            try
            {
                var review = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.IdReview == idReview);

                if (review == null) { return NotFound("Рецензия не найдена."); }

                review.StatusReview = status;

                await _context.SaveChangesAsync();

                return Ok("Статус рецензии обновлен.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Модератор")]
        [HttpPut("moder/update-collection-status/{idCollection}/{status}")]
        public async Task<ActionResult> UpdateCollectionStatusAsync(int idCollection, string status)
        {
            try
            {
                var collection = await _context.Collections
                    .FirstOrDefaultAsync(c => c.IdCollection == idCollection);

                if (collection == null) { return NotFound("Подборка не найдена."); }

                collection.StatusCollection = status;

                await _context.SaveChangesAsync();

                return Ok("Статус подборки обновлен.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Модератор")]
        [HttpPost("moder/add-violation")]
        public async Task<ActionResult> AddViolationAsync([FromBody] ViolationDTO model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.CategoryViolation) || string.IsNullOrEmpty(model.DescriptionViolation))
                {
                    return Conflict("Недостаточно данных.");
                }

                var newViolation = new Violation
                {
                    IdViolation = _context.Violations.Any() ? _context.Violations.Max(v => v.IdViolation) + 1 : 1,
                    IdUser = model.IdUser,
                    CategoryViolation = model.CategoryViolation,
                    DescriptionViolation = model.DescriptionViolation,
                    DateViolation = DateOnly.FromDateTime(DateTime.Now)
                };

                _context.Violations.Add(newViolation);

                await _context.SaveChangesAsync();

                return Ok("Нарушение добавлено.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Модератор")]
        [HttpGet("moder/comments")]
        public async Task<ActionResult> GetCommentsAsync()
        {
            try
            {
                var entitiesWithComments = await _context.Comments
                    .GroupBy(c => new { c.TypeEntity, c.IdEntity })
                    .Select(g => new
                    {
                        TypeEntity = g.Key.TypeEntity,
                        IdEntity = g.Key.IdEntity,
                        EntityName = g.Key.TypeEntity == "Рецензия" ?
                            _context.Reviews.Where(r => r.IdReview == g.Key.IdEntity).Select(r => r.TitleReview).FirstOrDefault() :
                            g.Key.TypeEntity == "Подборка" ?
                            _context.Collections.Where(col => col.IdCollection == g.Key.IdEntity).Select(col => col.TitleCollection).FirstOrDefault() :
                            "Неизвестный тип",
                        CommentCount = g.Count()
                    })
                    .ToListAsync();

                return Ok(entitiesWithComments);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Модератор")]
        [HttpGet("moder/comments/{entityType}/{idEnity}")]
        public async Task<ActionResult<List<CommentDTO>>> GetCommentsForEntityAsync(string entityType, int idEnity)
        {
            try
            {
                var allComments = await _context.Comments
                    .Where(c => c.TypeEntity == entityType && c.IdEntity == idEnity)
                    .Include(c => c.IdUserNavigation)
                    .ToListAsync();

                var comments = allComments
                    .Where(c => c.ParentCommentId == null)
                    .Select(c => MapComment(c, allComments))
                    .ToList();

                return Ok(comments);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        private CommentDTO MapComment(Comment comment, List<Comment> allComments)
        {
            return new CommentDTO
            {
                Id = comment.IdComment,
                Author = comment.IdUserNavigation.NameUser,
                AuthorURL = comment.IdUserNavigation.ProfileImageUrl,
                AuthorId = comment.IdUserNavigation.IdUser,
                Date = comment.DateComment,
                Content = comment.TextComment,
                IsReply = comment.ParentCommentId != null,
                Replies = allComments
                    .Where(r => r.ParentCommentId == comment.IdComment)
                    .Select(r => MapComment(r, allComments))
                    .ToList()
            };
        }

        [Authorize(Roles = "Модератор")]
        [HttpPost("moder/delete-comment/{idComment}")]
        public async Task<ActionResult> DeleteCommentWithViolationAsync(int idComment, [FromBody] ViolationDTO model)
        {
            try
            {
                var comment = await _context.Comments
                    .FirstOrDefaultAsync(c => c.IdComment == idComment);

                if (comment == null) { return NotFound("Комментарий не найден."); }

                _context.Comments.Remove(comment);

                if (string.IsNullOrEmpty(model.CategoryViolation) || string.IsNullOrEmpty(model.DescriptionViolation))
                {
                    return Conflict("Недостаточно данных.");
                }

                var newViolation = new Violation
                {
                    IdViolation = _context.Violations.Any() ? _context.Violations.Max(v => v.IdViolation) + 1 : 1,
                    IdUser = model.IdUser,
                    CategoryViolation = model.CategoryViolation,
                    DescriptionViolation = model.DescriptionViolation,
                    DateViolation = DateOnly.FromDateTime(DateTime.Now)
                };

                _context.Violations.Add(newViolation);

                await _context.SaveChangesAsync();

                return Ok("Нарушение добавлено.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Модератор")]
        [HttpGet("moder/users")]
        public async Task<ActionResult> GetUsersAsync()
        {
            try
            {
                var users = await _context.Users
                    .Where(u => u.Violations.Any())
                    .Select(u => new
                    {
                        IdUser = u.IdUser,
                        NameUser = u.NameUser,
                        LoginUser = u.LoginUser,
                        StatusUser = u.StatusUser,
                        CountViolations = u.Violations.Count
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Модератор")]
        [HttpGet("moder/user-violations/{idUser}")]
        public async Task<ActionResult> GetUserViolationsAsync(int idUser)
        {
            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.IdUser == idUser);

                if (existingUser == null) { return NotFound("Пользователь не найден."); }

                var userInfo = await _context.Users
                    .Where(u => u.IdUser == idUser)
                    .Select(u => new
                    {
                        IdUser = u.IdUser,
                        NameUser = u.NameUser,
                        LoginUser = u.LoginUser,
                        StatusUser = u.StatusUser,
                        CountViolations = u.Violations.Count
                    })
                    .FirstOrDefaultAsync();

                var violations = await _context.Violations
                    .Where(u => u.IdUser == idUser)
                    .Select(v => new
                    {
                        v.IdViolation,
                        v.CategoryViolation,
                        v.DescriptionViolation,
                        v.DateViolation
                    })
                    .ToListAsync();

                return Ok(new
                {
                    User = userInfo,
                    Violations = violations
                });
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Модератор")]
        [HttpPut("moder/change-status-user/{idUser}/{status}")]
        public async Task<ActionResult> ChangeStatusUserAsync(int idUser, string status)
        {
            try
            {
                var user = await _context.Users.
                    FirstOrDefaultAsync(u=>u.IdUser == idUser);

                if (user == null) { return NotFound("Пользователь не найден."); }

                user.StatusUser = status;

                await _context.SaveChangesAsync();

                return Ok("Статус пользователя изменен.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }
    }
}
