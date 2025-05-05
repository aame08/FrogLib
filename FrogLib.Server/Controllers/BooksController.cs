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
    public class BooksController(FroglibContext context, IBooksService service) : BaseController
    {
        private readonly FroglibContext _context = context;
        private readonly IBooksService _service = service;

        [HttpGet("all-books")]
        public async Task<ActionResult<List<BookDTO>>> GetAllBooksAsync()
        {
            try
            {
                var books = await _context.Books
                    .Include(b=>b.IdCategoryNavigation)
                    .ToListAsync();

                var allBooks = books.Select(book => new BookDTO
                {
                    Id = book.IdBook,
                    Title = book.TitleBook,
                    ImageURL = book.ImageUrl,
                    AverageRating = _service.GetAverageRatingAsync(book.IdBook).Result,
                    IdCategory = book.IdCategory,
                    YearPublication = book.YearPublication
                })
                .ToList();

                return Ok(allBooks);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [HttpGet("new-books")]
        public async Task<ActionResult<List<BookDTO>>> GetNewBooksAsync()
        {
            try
            {
                var books = await _context.Books
                    .Where(b => b.StatusBook == "Новинка")
                    .ToListAsync();

                var newBooks = books.Select(book => new BookDTO
                {
                    Id = book.IdBook,
                    Title = book.TitleBook,
                    ImageURL = book.ImageUrl,
                    AverageRating = _service.GetAverageRatingAsync(book.IdBook).Result
                })
                .ToList();

                return Ok(newBooks);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [HttpGet("best-selling")]
        public async Task<ActionResult<List<BookDTO>>> GetBestSellingAsync()
        {
            try
            {
                var books = await _context.Books
                    .Where(b => b.StatusBook == "Бестселлер")
                    .ToListAsync();

                var bestSellingBooks = books.Select(book => new BookDTO
                {
                    Id = book.IdBook,
                    Title = book.TitleBook,
                    ImageURL = book.ImageUrl,
                    AverageRating = _service.GetAverageRatingAsync(book.IdBook).Result
                })
                .ToList();

                return Ok(bestSellingBooks);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [HttpGet("popular-books")]
        public async Task<ActionResult<List<BookDTO>>> GetPopularBooksAsync()
        {
            try
            {
                var books = await _context.Books
                    .Include(b => b.Bookratings)
                    .Include(b => b.Userbooks)
                    .ToListAsync();

                var popularBooks = books.Select(book => new BookDTO
                {
                    Id = book.IdBook,
                    Title = book.TitleBook,
                    ImageURL = book.ImageUrl,
                    AverageRating = _service.GetAverageRatingAsync(book.IdBook).Result,
                    TotalRatings = book.Bookratings?.Count ?? 0,
                    TotalUserBookmarks = book.Userbooks?.Count ?? 0,
                    CountView = _service.GetCountViewAsync(book.IdBook).Result
                })
                .OrderByDescending(b => b.AverageRating)
                    .ThenByDescending(b => b.TotalRatings)
                    .ThenByDescending(b => b.TotalUserBookmarks)
                    .ThenByDescending(b => b.CountView)
                .Take(10)
                .ToList();

                return Ok(popularBooks);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Пользователь")]
        [HttpGet("recommendations/{idUser}")]
        public async Task<ActionResult<List<BookDTO>>> GetRecommendations(int idUser)
        {
            try
            {
                var response = await GetPythonRecommendations(idUser);

                var bookIds = response.Select(r => r.book_id).ToList();
                var booksFromDb = await _context.Books
                    .Where(b => bookIds.Contains(b.IdBook))
                    .ToListAsync();

                var result = booksFromDb.Select(book => new BookDTO
                {
                    Id = book.IdBook,
                    Title = book.TitleBook,
                    ImageURL = book.ImageUrl,
                    AverageRating = _service.GetAverageRatingAsync(book.IdBook).Result,
                    IdCategory = book.IdCategory,
                    YearPublication = book.YearPublication
                })
                .OrderBy(b => bookIds.IndexOf(b.Id))
                .ToList();

                return Ok(result);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<Category>>> GetCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .OrderBy(c => c.IdCategory)
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [HttpGet("list-types")]
        public async Task<ActionResult<List<Listtype>>> GetListTypesAsync()
        {
            try
            {
                var types = await _context.Listtypes
                    .OrderBy(t => t.IdListType)
                    .ToListAsync();

                return Ok(types);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [HttpGet("books/{idBook}")]
        public async Task<ActionResult> GetBookDetailsAsync(int idBook)
        {
            try
            {
                if (idBook <= 0) { return BadRequest("Идентификатор книги невалидный."); }

                var book = await _context.Books
                    .Include(b => b.IdPublisherNavigation)
                    .Include(b => b.IdCategoryNavigation)
                    .Include(b => b.IdAuthors)
                    .Include(b => b.Bookratings)
                    .Include(b => b.Reviews)
                        .ThenInclude(r => r.IdUserNavigation)
                    .Include(b => b.IdCollections)
                        .ThenInclude(c => c.IdBooks)
                    .Include(b => b.IdCollections)
                        .ThenInclude(c => c.IdUserNavigation)
                    .Include(b => b.Userbooks)
                        .ThenInclude(ub => ub.IdListTypeNavigation)
                    .FirstOrDefaultAsync(b => b.IdBook == idBook);

                if (book == null) { return NotFound("Книга не найдена."); }

                var totalRating = book.Bookratings?.Count ?? 0;
                var averageRating = await _service.GetAverageRatingAsync(book.IdBook);

                var ratingStatistics = Enumerable.Range(1, 5)
                    .Select(ratingValue => new
                    {
                        RatingValue = ratingValue,
                        Count = book.Bookratings?.Count(r => r.Rating == ratingValue) ?? 0,
                        Percentage = totalRating > 0 ? (double)book.Bookratings?.Count(r => r.Rating == ratingValue) / totalRating * 100 : 0
                    })
                    .OrderByDescending(r => r.RatingValue)
                    .ToList();

                var totalUserBookmarks = book.Userbooks?.Count ?? 0;
                var userBookmarks = new List<string> { "Читаю", "В планах", "Брошено", "Прочитано", "Любимое" }
                    .Select(listType => new
                    {
                        ListType = listType,
                        Count = book.Userbooks?.Count(ub => ub.IdListTypeNavigation?.NameList == listType) ?? 0,
                        Percentage = totalUserBookmarks > 0 ? (double)book.Userbooks?.Count(ub => ub.IdListTypeNavigation?.NameList == listType) / totalUserBookmarks * 100 : 0
                    })
                    .ToList();

                var countView = await _service.GetCountViewAsync(book.IdBook);
                var reviews = await _service.GetReviewsForBookAsync(book.IdBook);
                var collections = await _service.GetCollectionsForBookAsync(book.IdBook);
                var authorsFullName = await _service.GetAuthorsFullNameAsync(book.IdBook);

                return Ok(new
                {
                    ID = book.IdBook,
                    ISBN10 = book.Isbn10,
                    ISBN13 = book.Isbn13,
                    ImageURL = book.ImageUrl,
                    Title = book.TitleBook,
                    Description = book.DescriptionBook,
                    YearPublication = book.YearPublication,
                    PageCount = book.PageCount,
                    LanguageBook = book.LanguageBook,
                    Authors = authorsFullName,
                    Category = book.IdCategoryNavigation?.NameCategory,
                    Publisher = book.IdPublisherNavigation?.NamePublisher,
                    CountView = countView,
                    AverageRating = averageRating,
                    RatingStatistics = ratingStatistics,
                    TotalRatings = totalRating,
                    TotalUserBookmarks = totalUserBookmarks,
                    UserBookmarks = userBookmarks,
                    CountReviews = reviews?.Count ?? 0,
                    Reviews = reviews,
                    CountCollections = collections?.Count ?? 0,
                    Collections = collections
                });
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        private async Task<List<PythonBookRecommendation>> GetPythonRecommendations(int idUser)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"http://localhost:8000/recommend/{idUser}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<PythonBookRecommendation>();
            }

            return await response.Content.ReadFromJsonAsync<List<PythonBookRecommendation>>();
        }

        private record PythonBookRecommendation(int book_id, string title, string category);
    }
}
