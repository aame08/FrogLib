using FrogLib.Server.DTOs;
using FrogLib.Server.Models;
using FrogLib.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrogLib.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController(FroglibContext context, IReviewsService service) : BaseController
    {
        private readonly FroglibContext _context = context;
        private readonly IReviewsService _service = service;

        [HttpGet("all-reviews")]
        public async Task<ActionResult<List<ReviewDTO>>> GetAllReviewsAsync()
        {
            try
            {
                var reviews = await _context.Reviews
                    .Where(r => r.StatusReview == "Одобрено")
                    .Include(r => r.IdUserNavigation)
                    .Include(r => r.IdBookNavigation)
                    .ToListAsync();

                var allReviews = reviews.Select(r => new ReviewDTO
                {
                    Id = r.IdReview,
                    Title = r.TitleReview,
                    Content = r.TextReview,
                    ImageURL = r.IdBookNavigation.ImageUrl,
                    Rating = _service.GetRatingAsync(r.IdReview).Result.PositivePercent,
                    CountView = _service.GetCountViewAsync(r.IdReview).Result,
                    CreatedDate = r.CreatedDate,
                    UserName = r.IdUserNavigation.NameUser,
                    UserURL = r.IdUserNavigation.ProfileImageUrl
                })
                .OrderBy(r => r.Id)
                .ToList();

                return Ok(allReviews);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [HttpGet("popular-reviews")]
        public async Task<ActionResult<List<ReviewDTO>>> GetPopularReviewsAsync()
        {
            try
            {
                var reviews = await _context.Reviews
                    .Where(r => r.StatusReview == "Одобрено")
                    .Include(r => r.IdUserNavigation)
                    .Include(r => r.IdBookNavigation)
                    .ToListAsync();

                var popularReviews = reviews.Select(r => new ReviewDTO
                {
                    Id = r.IdReview,
                    Title = r.TitleReview,
                    Content = r.TextReview,
                    ImageURL = r.IdBookNavigation.ImageUrl,
                    Rating = _service.GetRatingAsync(r.IdReview).Result.PositivePercent,
                    CountView = _service.GetCountViewAsync(r.IdReview).Result,
                    UserName = r.IdUserNavigation.NameUser,
                    UserURL = r.IdUserNavigation.ProfileImageUrl
                })
                .OrderByDescending(r => r.Rating)
                    .ThenByDescending(r => r.CountView)
                .Take(10)
                .ToList();

                return Ok(popularReviews);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [HttpGet("reviews/{idReview}")]
        public async Task<ActionResult> GetReviewDetailsAsync(int idReview)
        {
            try
            {
                if (idReview <= 0) { return BadRequest("Идентификатор рецензии невалидный."); }

                var review = await _context.Reviews
                    .Include(r => r.IdUserNavigation)
                    .Include(r => r.IdBookNavigation)
                    .FirstOrDefaultAsync(r => r.IdReview == idReview);

                if (review == null) { return NotFound("Рецензия не найдена."); }

                var author = review.IdUserNavigation.IdUser;

                var ratingInfo = await _service.GetRatingAsync(review.IdReview);

                var countView = await _service.GetCountViewAsync(review.IdReview);

                var book = await _service.GetBookForReviewAsync(review.IdReview);

                var bookRating = await _service.GetBookRatingAsync(book.Id, author);

                var comments = await _service.GetCommentsForReviewAsync(review.IdReview);

                return Ok(new
                {
                    ID = review.IdReview,
                    Title = review.TitleReview,
                    Content = review.TextReview,
                    Rating = ratingInfo.PositivePercent,
                    Likes = ratingInfo.Likes,
                    Dislikes = ratingInfo.Dislikes,
                    CountView = countView,
                    CreatedDate = review.CreatedDate,
                    UserId = review.IdUserNavigation.IdUser,
                    UserName = review.IdUserNavigation.NameUser,
                    UserURL = review.IdUserNavigation.ProfileImageUrl,
                    Book = book,
                    BookRating = bookRating,
                    Comments = comments
                });
            }
            catch (Exception ex) { return HandleException(ex); }
        }
    }
}
