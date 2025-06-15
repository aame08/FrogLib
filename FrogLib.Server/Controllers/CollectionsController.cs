using FrogLib.Server.DTOs;
using FrogLib.Server.Models;
using FrogLib.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrogLib.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionsController(FroglibContext context, ICollectionsService service) : BaseController
    {
        private readonly FroglibContext _context = context;
        private readonly ICollectionsService _service = service;

        [HttpGet("all-collections")]
        public async Task<ActionResult<List<CollectionDTO>>> GetAllCollectionsAsync()
        {
            try
            {
                var collections = await _context.Collections
                    .Where(c => c.StatusCollection == "Одобрено")
                    .Include(c => c.Likedcollections)
                    .Include(c => c.IdUserNavigation)
                    .Include(c => c.IdBooks)
                    .ToListAsync();

                var allCollections = collections.Select(c => new CollectionDTO
                {
                    Id = c.IdCollection,
                    Title = c.TitleCollection,
                    Description = c.DescriptionCollection,
                    Rating = _service.GetRatingAsync(c.IdCollection).Result.PositivePercent,
                    CountBooks = c.IdBooks.Count,
                    Books = c.IdBooks
                        .Take(3)
                        .Select(bc => new BookDTO
                        {
                            Id = bc.IdBook,
                            Title = bc.TitleBook,
                            ImageURL = bc.ImageUrl
                        })
                        .ToList(),
                    CountView = _service.GetCountViewAsync(c.IdCollection).Result,
                    CountComments = _service.GetCountCommentsAsync(c.IdCollection).Result,
                    CountLiked = c.Likedcollections.Count,
                    CreatedDate = c.CreatedDate,
                    UserName = c.IdUserNavigation.NameUser,
                    UserURL = c.IdUserNavigation.ProfileImageUrl
                })
                .OrderBy(c => c.Id)
                .ToList();

                return Ok(allCollections);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [HttpGet("popular-collections")]
        public async Task<ActionResult<List<CollectionDTO>>> GetPopularCollectionsAsync()
        {
            try
            {
                var collections = await _context.Collections
                    .Where(c => c.StatusCollection == "Одобрено")
                    .Include(c => c.IdUserNavigation)
                    .Include(c => c.Likedcollections)
                    .Include(c => c.IdBooks)
                    .ToListAsync();

                var popularCollections = new List<CollectionDTO>();
                foreach (var c in collections)
                {
                    var rating = await _service.GetRatingAsync(c.IdCollection);
                    var countView = await _service.GetCountViewAsync(c.IdCollection);
                    var countComments = await _service.GetCountCommentsAsync(c.IdCollection);

                    popularCollections.Add(new CollectionDTO
                    {
                        Id = c.IdCollection,
                        Title = c.TitleCollection,
                        Description = c.DescriptionCollection,
                        Rating = rating.PositivePercent,
                        CountBooks = c.IdBooks.Count,
                        Books = c.IdBooks
                            .Take(3)
                            .Select(bc => new BookDTO
                            {
                                Id = bc.IdBook,
                                Title = bc.TitleBook,
                                ImageURL = bc.ImageUrl
                            })
                            .ToList(),
                        CountView = countView,
                        CountComments = countComments,
                        CountLiked = c.Likedcollections.Count,
                        UserName = c.IdUserNavigation.NameUser,
                        UserURL = c.IdUserNavigation.ProfileImageUrl
                    });
                }

                var sortedCollections = popularCollections
                        .OrderByDescending(c => c.Rating)
                            .ThenByDescending(c => c.CountComments)
                            .ThenByDescending(c => c.CountView)
                        .Take(10)
                        .ToList();

                return Ok(sortedCollections);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [HttpGet("collections/{idCollection}")]
        public async Task<ActionResult> GetCollectionDetailsAsync(int idCollection)
        {
            try
            {
                if (idCollection <= 0) { return BadRequest("Идентификатор подборки невалидный."); }

                var collection = await _context.Collections
                    .Include(c => c.Likedcollections)
                    .Include(c => c.IdUserNavigation)
                    .Include(c => c.IdBooks)
                    .FirstOrDefaultAsync(c => c.IdCollection == idCollection);

                if (collection == null) { return NotFound("Подборка не найдена."); }

                var ratingInfo = await _service.GetRatingAsync(collection.IdCollection);

                var countView = await _service.GetCountViewAsync(collection.IdCollection);

                var comments = await _service.GetCommentsForCollectionAsync(collection.IdCollection);

                var books = await _service.GetBooksForCollectionAsync(collection.IdCollection);

                return Ok(new
                {
                    ID = collection.IdCollection,
                    Title = collection.TitleCollection,
                    Description = collection.DescriptionCollection,
                    Rating = ratingInfo.PositivePercent,
                    Likes = ratingInfo.Likes,
                    Dislikes = ratingInfo.Dislikes,
                    CountBooks = collection.IdBooks.Count,
                    CountView = countView,
                    CountLiked = collection.Likedcollections.Count,
                    CreatedDate = collection.CreatedDate,
                    UserId = collection.IdUserNavigation.IdUser,
                    UserName = collection.IdUserNavigation.NameUser,
                    UserURL = collection.IdUserNavigation.ProfileImageUrl,
                    Books = books,
                    Comments = comments
                });
            }
            catch (Exception ex) { return HandleException(ex); }
        }
    }
}
