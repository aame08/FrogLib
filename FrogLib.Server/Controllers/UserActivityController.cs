using FrogLib.Server.DTOs;
using FrogLib.Server.Models;
using FrogLib.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrogLib.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserActivityController(FroglibContext context) : BaseController
    {
        private readonly FroglibContext _context = context;

        [Authorize(Roles = "Пользователь")]
        [HttpPost("add-view/{idUser}/{idEntity}/{typeObject}")]
        public async Task<ActionResult> AddViewAsync(int idUser, int idEntity, string typeObject)
        {
            try
            {
                if (idUser <= 0 || idEntity <= 0 || string.IsNullOrEmpty(typeObject))
                {
                    return Conflict("Данных не хватает.");
                }

                var existingView = await _context.Viewhistories
                    .FirstOrDefaultAsync(vh => vh.IdUser == idUser && vh.IdEntity == idEntity && vh.TypeEntity == typeObject);

                if (existingView == null)
                {
                    var newView = new Viewhistory
                    {
                        IdHistory = _context.Viewhistories.Any() ? _context.Viewhistories.Max(vh => vh.IdHistory) + 1 : 1,
                        IdUser = idUser,
                        IdEntity = idEntity,
                        TypeEntity = typeObject,
                        ViewDate = DateOnly.FromDateTime(DateTime.Now)
                    };
                    _context.Viewhistories.Add(newView);
                    await _context.SaveChangesAsync();

                    return Ok("Просмотр добавлен.");
                }

                return Ok("Просмотр уже существует.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpGet("get-book-rating/{idUser}/{idBook}")]
        public async Task<ActionResult<int>> GetUserBookRatingAsync(int idUser, int idBook)
        {
            try
            {
                if (idUser <= 0 || idBook <= 0) { return Conflict("Данных не хватает"); }

                var bookRating = await _context.Bookratings
                    .FirstOrDefaultAsync(br => br.IdUser == idUser && br.IdBook == idBook);

                if (bookRating == null) { return 0; }

                return bookRating.Rating;
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpPost("update-book-rating/{idUser}/{idBook}/{newRating}")]
        public async Task<ActionResult> UpdateBookRatingAsync(int idUser, int idBook, int newRating)
        {
            try
            {
                if (idUser <= 0 || idBook <= 0 || newRating <= 0) { return Conflict("Данных не хватает"); }

                var userRating = await _context.Bookratings
                    .FirstOrDefaultAsync(r => r.IdUser == idUser && r.IdBook == idBook);

                if (userRating != null)
                {
                    userRating.Rating = newRating;
                }
                else
                {
                    var newBookRating = new Bookrating
                    {
                        IdRating = _context.Bookratings.Any() ? _context.Bookratings.Max(u => u.IdRating) + 1 : 1,
                        IdUser = idUser,
                        IdBook = idBook,
                        Rating = newRating
                    };
                    _context.Bookratings.Add(newBookRating);
                }

                await _context.SaveChangesAsync();

                return Ok("Рейтинг добавлен/изменен.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpDelete("delete-book-rating/{idUser}/{idBook}")]
        public async Task<ActionResult> DeleteBookRatingAsync(int idUser, int idBook)
        {
            try
            {
                var deletingRating = await _context.Bookratings
                    .FirstOrDefaultAsync(r => r.IdUser == idUser && r.IdBook == idBook);

                if (deletingRating != null)
                {
                    _context.Remove(deletingRating);

                    await _context.SaveChangesAsync();

                    return Ok("Рейтинг удален.");
                }

                return NotFound("Рейтинг не найден.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpGet("get-entity-rating/{idUser}/{idEntity}/{typeEntity}")]
        public async Task<ActionResult<int>> GetEntityRatingAsync(int idUser, int idEntity, string typeEntity)
        {
            try
            {
                var rating = await _context.Entityratings
                    .FirstOrDefaultAsync(r => r.IdUser == idUser && r.IdEntity == idEntity && r.TypeEntity == typeEntity);

                if (rating == null) { return -1; }

                return rating.Rating;
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpPost("update-entity-rating/{idUser}/{idEntity}/{typeEntity}/{newRating}")]
        public async Task<ActionResult> UpdateEntityRatingAsync(int idUser, int idEntity, string typeEntity, int newRating)
        {
            try
            {
                var rating = await _context.Entityratings
                    .FirstOrDefaultAsync(r => r.IdUser == idUser && r.IdEntity == idEntity && r.TypeEntity == typeEntity);

                if (rating != null)
                {
                    rating.Rating = Convert.ToSByte(newRating);
                }
                else
                {
                    var newRatingEntity = new Entityrating
                    {
                        IdRating = _context.Entityratings.Any() ? _context.Entityratings.Max(u => u.IdRating) + 1 : 1,
                        IdUser = idUser,
                        IdEntity = idEntity,
                        TypeEntity = typeEntity,
                        Rating = Convert.ToSByte(newRating)
                    };

                    _context.Entityratings.Add(newRatingEntity);
                }

                await _context.SaveChangesAsync();

                return Ok("Рейтинг добавлен.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpGet("get-user-list/{idUser}/{idBook}")]
        public async Task<ActionResult<int>> GetUserListAsync(int idUser, int idBook)
        {
            try
            {
                var list = await _context.Userbooks
                    .FirstOrDefaultAsync(l => l.IdUser == idUser && l.IdBook == idBook);

                if (list == null) { return 0; }

                return list.IdListType;
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpPost("update-user-list/{idUser}/{idBook}/{idList}")]
        public async Task<ActionResult> UpdateUserListAsync(int idUser, int idBook, int idList)
        {
            try
            {
                var userList = await _context.Userbooks
                    .FirstOrDefaultAsync(l => l.IdUser == idUser && l.IdBook == idBook);

                if (userList != null)
                {
                    if (userList.IdListType == idList)
                    {
                        return Ok("Книга уже добавлена.");
                    }
                    else
                    {
                        _context.Userbooks.Remove(userList);
                    }
                }
                var newUserList = new Userbook
                {
                    IdListType = idList,
                    IdBook = idBook,
                    IdUser = idUser,
                    AddedDate = DateOnly.FromDateTime(DateTime.Now)
                };

                _context.Userbooks.Add(newUserList);

                await _context.SaveChangesAsync();

                return Ok("Книга добавлена.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpDelete("delete-user-list/{idUser}/{idBook}")]
        public async Task<ActionResult> DeleteUserListAsync(int idUser, int idBook)
        {
            try
            {
                var userList = await _context.Userbooks
                    .FirstOrDefaultAsync(l => l.IdUser == idUser && l.IdBook == idBook);

                if (userList == null) { return NotFound("Запись не найдена."); }

                _context.Userbooks.Remove(userList);
                await _context.SaveChangesAsync();

                return Ok("Книга удалена.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpGet("get-liked-collection/{idUser}/{idCollection}")]
        public async Task<ActionResult<bool>> GetLikedCollectionAsync(int idUser, int idCollection)
        {
            try
            {
                var liked = await _context.Likedcollections
                    .FirstOrDefaultAsync(l => l.IdUser == idUser && l.IdCollection == idCollection);

                if (liked == null) { return false; }

                return true;
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpPost("like-collection/{idUser}/{idCollection}")]
        public async Task<ActionResult> LikeCollectionAsync(int idUser, int idCollection)
        {
            try
            {
                var existingCollection = await _context.Collections
                    .FirstOrDefaultAsync(c => c.IdCollection == idCollection);

                if (existingCollection.IdUser == idUser)
                {
                    return Conflict("Автор не может сохранить свою подборку.");
                }

                var likedCollection = new Likedcollection
                {
                    IdCollection = idCollection,
                    IdUser = idUser,
                    LikedDate = DateOnly.FromDateTime(DateTime.Now)
                };

                _context.Likedcollections.Add(likedCollection);
                await _context.SaveChangesAsync();

                return Ok("Коллекция добавлена в Избранные.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpDelete("unlike-collection/{idUser}/{idCollection}")]
        public async Task<ActionResult> UnlikeCollectionAsync(int idUser, int idCollection)
        {
            try
            {
                var unlikedCollection = await _context.Likedcollections
                    .FirstOrDefaultAsync(c => c.IdUser == idUser && c.IdCollection == idCollection);

                _context.Likedcollections.Remove(unlikedCollection);
                await _context.SaveChangesAsync();

                return Ok("Коллекция удалена.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpPost("add-new-comment/{idUser}/{idEntity}/{typeEntity}/{textComment}")]
        public async Task<ActionResult> NewCommentAsync(int idUser, int idEntity, string typeEntity, string textComment, [FromServices] ContentModerationService moderationService)
        {
            try
            {
                if (idUser <= 0 || idEntity <= 0 || string.IsNullOrEmpty(typeEntity) || string.IsNullOrEmpty(textComment)) { return Conflict("Данных не хватает"); }

                var status = moderationService.ContainsForbiddenWords(textComment) ? "Обнаружено нарушение" : "Новое";

                var newComment = new Comment
                {
                    IdComment = _context.Comments.Any() ? _context.Comments.Max(c => c.IdComment) + 1 : 1,
                    IdUser = idUser,
                    IdEntity = idEntity,
                    TypeEntity = typeEntity,
                    TextComment = textComment,
                    DateComment = DateOnly.FromDateTime(DateTime.Now),
                    ParentCommentId = null,
                    StatusComment = status
                };

                _context.Comments.Add(newComment);
                await _context.SaveChangesAsync();

                return Ok("Комментарий добавлен.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpPost("add-reply-comment/{idUser}/{idEntity}/{typeEntity}/{textComment}/{parentId}")]
        public async Task<ActionResult> ReplyCommentAsync(int idUser, int idEntity, string typeEntity, string textComment, int parentId, [FromServices] ContentModerationService moderationService)
        {
            try
            {
                if (idUser <= 0 || idEntity <= 0 || parentId <= 0 || string.IsNullOrEmpty(typeEntity) || string.IsNullOrEmpty(textComment)) { return Conflict("Данных не хватает"); }

                var status = moderationService.ContainsForbiddenWords(textComment) ? "Обнаружено нарушение" : "Новое";

                var newReplyComment = new Comment
                {
                    IdComment = _context.Comments.Any() ? _context.Comments.Max(c => c.IdComment) + 1 : 1,
                    IdUser = idUser,
                    IdEntity = idEntity,
                    TypeEntity = typeEntity,
                    TextComment = textComment,
                    DateComment = DateOnly.FromDateTime(DateTime.Now),
                    ParentCommentId = parentId,
                    StatusComment = status
                };

                _context.Comments.Add(newReplyComment);
                await _context.SaveChangesAsync();

                return Ok("Ответ на комментарий добавлен.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpPost("add-review")]
        public async Task<ActionResult> AddReviewAsync([FromBody] ReviewRequest request, [FromServices] ContentModerationService moderationService)
        {
            try
            {
                var existingRating = await _context.Bookratings
                    .FirstOrDefaultAsync(r => r.IdUser == request.IdUser && r.IdBook == request.IdBook);

                if (existingRating != null && existingRating.Rating != request.NewRating)
                {
                    existingRating.Rating = request.NewRating;
                }
                else
                {
                    var newBookRating = new Bookrating
                    {
                        IdRating = _context.Bookratings.Any() ? _context.Bookratings.Max(r => r.IdRating) + 1 : 1,
                        IdUser = request.IdUser,
                        IdBook = request.IdBook,
                        Rating = request.NewRating
                    };

                    _context.Bookratings.Add(newBookRating);
                }

                var status = "На рассмотрении";
                if (moderationService.ContainsForbiddenWords(request.TitleReview) || moderationService.ContainsForbiddenWords(request.TextReview))
                {
                    status = "Обнаружено нарушение";
                }

                var newReview = new Review
                {
                    IdReview = _context.Reviews.Any() ? _context.Reviews.Max(r => r.IdReview) + 1 : 1,
                    IdUser = request.IdUser,
                    IdBook = request.IdBook,
                    TitleReview = request.TitleReview,
                    TextReview = request.TextReview,
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                    StatusReview = status
                };

                _context.Reviews.Add(newReview);

                await _context.SaveChangesAsync();

                return Ok("Рецензия отправлена на рассмотрение.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpPut("edit-review")]
        public async Task<ActionResult> EditReviewAsync(int idReview, [FromBody] ReviewRequest request, [FromServices] ContentModerationService moderationService)
        {
            try
            {
                var existingReview = await _context.Reviews.FindAsync(idReview);

                if (existingReview == null) { return NotFound("Рецензия не найдена."); }

                var existingRating = await _context.Bookratings
                    .FirstOrDefaultAsync(r => r.IdUser == request.IdUser && r.IdBook == request.IdBook);

                if (existingRating != null)
                {
                    existingRating.Rating = request.NewRating;
                }
                else
                {
                    var newBookRating = new Bookrating
                    {
                        IdRating = _context.Bookratings.Any() ? _context.Bookratings.Max(r => r.IdRating) + 1 : 1,
                        IdUser = request.IdUser,
                        IdBook = request.IdBook,
                        Rating = request.NewRating
                    };

                    _context.Bookratings.Add(newBookRating);
                }

                var status = "На рассмотрении";
                if (moderationService.ContainsForbiddenWords(request.TitleReview) || moderationService.ContainsForbiddenWords(request.TextReview))
                {
                    status = "Обнаружено нарушение";
                }

                existingReview.TitleReview = request.TitleReview;
                existingReview.TextReview = request.TextReview;
                existingReview.StatusReview = status;

                await _context.SaveChangesAsync();

                return Ok("Рецензия обновлена.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь, Администратор")]
        [HttpPost("add-collection")]
        public async Task<ActionResult> AddCollectionAsync([FromBody] CollectionRequest request, [FromServices] ContentModerationService moderationService)
        {
            try
            {
                string statusCollection = "На рассмотрении";

                var user = await _context.Users.FindAsync(request.IdUser);

                if (user.NameRole == "Администратор") { statusCollection = "Одобрено"; }
                else if (moderationService.ContainsForbiddenWords(request.TitleCollection) || moderationService.ContainsForbiddenWords(request.DescriptionCollection))
                {
                    statusCollection = "Обнаружено нарушение";
                }

                var books = await _context.Books
                    .Where(b => request.IdBooks.Contains(b.IdBook))
                    .ToListAsync();

                if (books.Count < 5) { return Conflict("В подборке должно быть от 5 книг."); }

                var newCollection = new Collection
                {
                    IdCollection = _context.Collections.Any() ? _context.Collections.Max(c => c.IdCollection) + 1 : 1,
                    IdUser = request.IdUser,
                    TitleCollection = request.TitleCollection,
                    DescriptionCollection = request.DescriptionCollection,
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                    StatusCollection = statusCollection,
                    IdBooks = books
                };

                _context.Collections.Add(newCollection);

                await _context.SaveChangesAsync();

                return Ok("Подборка отправлена на рассмотрение.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь, Администратор")]
        [HttpPut("edit-collection/{idCollection}")]
        public async Task<ActionResult> EditCollectionAsync(int idCollection, [FromBody] CollectionRequest request, [FromServices] ContentModerationService moderationService)
        {
            try
            {
                string statusCollection = "На рассмотрении";

                var user = await _context.Users.FindAsync(request.IdUser);

                if (user.NameRole == "Администратор") { statusCollection = "Одобрено"; }
                else if (moderationService.ContainsForbiddenWords(request.TitleCollection) || moderationService.ContainsForbiddenWords(request.DescriptionCollection))
                {
                    statusCollection = "Обнаружено нарушение";
                }

                var existingCollection = await _context.Collections
                    .Include(b => b.IdBooks)
                    .FirstOrDefaultAsync(c => c.IdCollection == idCollection);

                if (existingCollection == null) { return NotFound("Подборка не найдена."); }

                existingCollection.TitleCollection = request.TitleCollection;
                existingCollection.DescriptionCollection = request.DescriptionCollection;
                existingCollection.StatusCollection = statusCollection;

                existingCollection.IdBooks.Clear();

                var books = await _context.Books
                    .Where(b => request.IdBooks.Contains(b.IdBook))
                    .ToListAsync();

                if (books.Count < 5) { return Conflict("В подборке должно быть от 5 книг."); }

                existingCollection.IdBooks = books;

                await _context.SaveChangesAsync();

                return Ok("Подборка обновлена.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь, Администратор")]
        [HttpGet("user-collections/{IdUser}")]
        public async Task<ActionResult> GetUserCollectionsAsync(int IdUser)
        {
            try
            {
                var collections = await _context.Collections
                    .Where(c => c.IdUser == IdUser)
                    .Include(b => b.IdBooks)
                    .Select(c => new
                    {
                        c.IdCollection,
                        c.TitleCollection,
                        c.DescriptionCollection,
                        CountBooks = c.IdBooks.Count,
                        Books = c.IdBooks.Take(3)
                            .Select(b => new BookDTO
                            {
                                Id = b.IdBook,
                                Title = b.TitleBook,
                                ImageURL = b.ImageUrl
                            })
                            .ToList(),
                        c.StatusCollection
                    })
                    .ToListAsync();

                return Ok(collections);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpGet("user-liked-collections/{idUser}")]
        public async Task<ActionResult> GetUserLikedCollectionsAsync(int idUser)
        {
            try
            {
                var likedCollections = await _context.Likedcollections
                    .Where(lc => lc.IdUser == idUser && lc.IdCollectionNavigation.StatusCollection == "Одобрено")
                    .Select(lc => new
                    {
                        lc.IdCollectionNavigation.IdCollection,
                        lc.IdCollectionNavigation.TitleCollection,
                        lc.IdCollectionNavigation.DescriptionCollection,
                        CountBooks = lc.IdCollectionNavigation.IdBooks.Count,
                        Books = lc.IdCollectionNavigation.IdBooks
                            .Take(3)
                            .Select(b => new BookDTO
                            {
                                Id = b.IdBook,
                                Title = b.TitleBook,
                                ImageURL = b.ImageUrl
                            })
                            .ToList()
                    })
                    .ToListAsync();

                return Ok(likedCollections);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpGet("user-books/{idUser}")]
        public async Task<ActionResult> GetUserBooksAsync(int idUser)
        {
            try
            {
                var books = await _context.Userbooks
                    .Where(b => b.IdUser == idUser)
                    .Include(b => b.IdListTypeNavigation)
                    .Include(b => b.IdBookNavigation)
                    .Select(b => new
                    {
                        IDBook = b.IdBook,
                        ImageURL = b.IdBookNavigation.ImageUrl,
                        TitleBook = b.IdBookNavigation.TitleBook,
                        AddedDate = b.AddedDate,
                        Rating = _context.Bookratings
                            .FirstOrDefault(r => r.IdUser == b.IdUser && r.IdBook == b.IdBook) != null
                            ? _context.Bookratings
                            .FirstOrDefault(r => r.IdUser == b.IdUser && r.IdBook == b.IdBook)
                            .Rating : (int?)null,
                        IDListType = b.IdListType,
                        NameListType = b.IdListTypeNavigation.NameList
                    })
                    .ToListAsync();

                return Ok(books);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpGet("user-comments/{idUser}")]
        public async Task<ActionResult> GetUserCommentsAsync(int idUser)
        {
            try
            {
                var comments = await _context.Comments
                    .Where(c => c.IdUser == idUser)
                    .Include(c => c.IdUserNavigation)
                    .Select(c => new
                    {
                        IDComment = c.IdComment,
                        ContentComment = c.TextComment,
                        DateComment = c.DateComment,
                        TypeEntity = c.TypeEntity,
                        IDEntity = c.IdEntity,
                        EntityName = c.TypeEntity == "Рецензия" ?
                            _context.Reviews.Where(r => r.IdReview == c.IdEntity).Select(r => r.TitleReview).FirstOrDefault() :
                            c.TypeEntity == "Подборка" ?
                            _context.Collections.Where(col => col.IdCollection == c.IdEntity).Select(col => col.TitleCollection).FirstOrDefault() :
                            "Неизвестный тип"
                    })
                    .ToListAsync();

                return Ok(comments);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpPut("update-comment/{idUser}/{idComment}/{textComment}")]
        public async Task<ActionResult> UpdateCommentAsync(int idUser, int idComment, string textComment, [FromServices] ContentModerationService moderationService)
        {
            try
            {
                var comment = await _context.Comments
                    .FirstOrDefaultAsync(c => c.IdComment == idComment && c.IdUser == idUser);

                var status = moderationService.ContainsForbiddenWords(textComment) ? "Обнаружено нарушение" : "Новое";

                comment.TextComment = textComment;
                comment.DateComment = DateOnly.FromDateTime(DateTime.Now);
                comment.StatusComment = status;

                await _context.SaveChangesAsync();

                return Ok("Комментарий обновлен.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpDelete("delete-comment/{idUser}/{idComment}")]
        public async Task<ActionResult> DeleteCommentAsync(int idUser, int idComment)
        {
            try
            {
                var comment = await _context.Comments
                    .Include(c => c.InverseParentComment)
                    .FirstOrDefaultAsync(c => c.IdComment == idComment && c.IdUser == idUser);

                if (comment.InverseParentComment != null && comment.InverseParentComment.Any())
                {
                    _context.Comments.RemoveRange(comment.InverseParentComment);
                }

                _context.Comments.Remove(comment);

                await _context.SaveChangesAsync();

                return Ok("Комментарий удален.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpGet("user-reviews/{idUser}")]
        public async Task<ActionResult> GetUserReviewsAsync(int idUser)
        {
            try
            {
                var reviews = await _context.Reviews
                    .Where(r => r.IdUser == idUser)
                    .Include(r => r.IdBookNavigation)
                    .Select(r => new
                    {
                        r.IdReview,
                        r.TitleReview,
                        r.TextReview,
                        r.CreatedDate,
                        IDBook = r.IdBookNavigation.IdBook,
                        TitleBook = r.IdBookNavigation.TitleBook,
                        ImageURL = r.IdBookNavigation.ImageUrl,
                        Rating = _context.Bookratings
                            .Where(br => br.IdUser == idUser && br.IdBook == r.IdBookNavigation.IdBook)
                            .Select(br => (int?)br.Rating)
                            .FirstOrDefault(),
                        r.StatusReview
                    })
                    .ToListAsync();

                return Ok(reviews);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpGet("user-calendar/{idUser}")]
        public async Task<ActionResult> GetUserCalendarAsync(int idUser)
        {
            try
            {
                var calendar = await _context.Userbooks
                    .Where(ub => ub.IdUser == idUser)
                    .Include(ub => ub.IdBookNavigation)
                    .Include(ub => ub.IdListTypeNavigation)
                    .Select(ub => new
                    {
                        IDBook = ub.IdBook,
                        Title = ub.IdBookNavigation.TitleBook,
                        ImageURL = ub.IdBookNavigation.ImageUrl,
                        AddedDate = ub.AddedDate,
                        ListType = new
                        {
                            Id = ub.IdListTypeNavigation.IdListType,
                            Name = ub.IdListTypeNavigation.NameList
                        }
                    })
                    .ToListAsync();

                return Ok(calendar);
            }
            catch (Exception ex) { return HandleException(ex); }
        }
    }
}
