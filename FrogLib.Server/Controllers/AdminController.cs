using FrogLib.Server.DTOs;
using FrogLib.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FrogLib.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(FroglibContext context) : BaseController
    {
        private readonly FroglibContext _context = context;

        [Authorize(Roles = "Администратор")]
        [HttpGet("admin/all-books")]
        public async Task<ActionResult<List<AdminBookDTO>>> GetAdminBooksAsync()
        {
            try
            {
                var books = await _context.Books
                    .Include(b => b.IdAuthors)
                    .Include(b => b.IdCategoryNavigation)
                    .Include(b => b.IdPublisherNavigation)
                    .Select(book => new AdminBookDTO
                    {
                        Id = book.IdBook,
                        Isbn10 = book.Isbn10,
                        Isbn13 = book.Isbn13,
                        PublisherName = book.IdPublisherNavigation.NamePublisher,
                        CategoryName = book.IdCategoryNavigation.NameCategory,
                        ImageUrl = book.ImageUrl,
                        TitleBook = book.TitleBook,
                        DescriptionBook = book.DescriptionBook,
                        YearPublication = book.YearPublication,
                        PageCount = book.PageCount,
                        LanguageBook = book.LanguageBook,
                        StatusBook = book.StatusBook,
                        Authors = book.IdAuthors.Select(a => new AuthorDTO
                        {
                            SurnameAuthor = a.SurnameAuthor,
                            NameAuthor = a.NameAuthor,
                            PatronymicAuthor = a.PatronymicAuthor
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(books);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet("admin/search")]
        public async Task<ActionResult> SearchBooksAsync([FromQuery] string title)
        {
            try
            {
                if (string.IsNullOrEmpty(title)) { return Conflict("Название не может быть пустым."); }
                string apiKey = "AIzaSyBP0RdShwD1NnK47iEBl6-yCe8Lf8EAnkI";
                string requestUri = $"https://www.googleapis.com/books/v1/volumes?q={title}&key={apiKey}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(requestUri);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        JObject bookData = JObject.Parse(jsonResponse);

                        var items = bookData["items"] as JArray;
                        if (items != null && items.Count > 0)
                        {
                            var result = new List<BookSearchResult>();

                            foreach (var item in items.Take(5))
                            {
                                var volumeInfo = item["volumeInfo"];
                                if (volumeInfo == null) { continue; }

                                var description = volumeInfo["description"]?.ToString();
                                if (string.IsNullOrWhiteSpace(description)) { continue; }

                                var book = new BookSearchResult
                                {
                                    Title = volumeInfo["title"]?.ToString(),
                                    Authors = volumeInfo["authors"]?.ToObject<List<string>>() ?? new List<string>(),
                                    Publisher = volumeInfo["publisher"]?.ToString(),
                                    YearPublication = ParseYear(volumeInfo["publishedDate"]?.ToString()),
                                    Description = description,
                                    PageCount = volumeInfo["pageCount"]?.ToObject<int?>(),
                                    Language = volumeInfo["language"]?.ToString(),
                                    Categories = TranslateCategories(volumeInfo["categories"]?.ToObject<List<string>>() ?? new List<string>())
                                };

                                var industryIdentifiers = volumeInfo["industryIdentifiers"] as JArray;

                                if (industryIdentifiers != null)
                                {
                                    foreach (var identifier in industryIdentifiers)
                                    {
                                        string type = identifier["type"]?.ToString();
                                        string id = identifier["identifier"]?.ToString();

                                        if (type == "ISBN_10") book.Isbn10 = id;
                                        else if (type == "ISBN_13") book.Isbn13 = id;
                                    }
                                }

                                var imageLinks = volumeInfo["imageLinks"];
                                if (imageLinks != null)
                                {
                                    string thumbnailUrl = imageLinks["thumbnail"]?.ToString();
                                    book.ImageUrl = GetHighQualityImageUrl(thumbnailUrl, zoomLevel: 3);
                                }

                                result.Add(book);
                            }

                            return Ok(result);
                        }
                    }
                    return NotFound("Книги не найдены.");
                }
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost("admin/add-book")]
        public async Task<ActionResult> AddBookAsync([FromBody] AddBookDTO book)
        {
            try
            {
                var publisher = await GetOrCreatePublisherAsync(book.PublisherName);

                var category = await GetOrCreateCategoryAsync(book.CategoryName);

                var authors = new List<Author>();
                foreach (var authorDTO in book.Authors)
                {
                    var author = await GetOrCreateAuthorAsync(authorDTO);
                    authors.Add(author);
                }

                var newBook = new Book
                {
                    IdBook = _context.Books.Any() ? _context.Books.Max(b => b.IdBook) + 1 : 1,
                    Isbn10 = book.Isbn10,
                    Isbn13 = book.Isbn13,
                    IdPublisher = publisher.IdPublisher,
                    IdCategory = category.IdCategory,
                    ImageUrl = book.ImageUrl,
                    TitleBook = book.TitleBook,
                    DescriptionBook = book.DescriptionBook,
                    YearPublication = book.YearPublication,
                    PageCount = book.PageCount,
                    LanguageBook = book.LanguageBook,
                    AddedDate = DateOnly.FromDateTime(DateTime.Now),
                    StatusBook = book.StatusBook,
                    IdAuthors = authors
                };

                _context.Books.Add(newBook);
                await _context.SaveChangesAsync();

                return Ok("Книга добавлена.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Администратор")]
        [HttpPut("admin/edit-book/{idBook}")]
        public async Task<ActionResult> EditBookAsync(int idBook, [FromBody] AddBookDTO book)
        {
            try
            {
                var existingBook = await _context.Books
                    .Include(b => b.IdCategoryNavigation)
                    .Include(b => b.IdPublisherNavigation)
                    .Include(b => b.IdAuthors)
                    .FirstOrDefaultAsync(b => b.IdBook == idBook);

                if (existingBook == null) { return NotFound("Книга не найдена."); }

                existingBook.Isbn10 = book.Isbn10;
                existingBook.Isbn13 = book.Isbn13;
                existingBook.TitleBook = book.TitleBook;
                existingBook.DescriptionBook = book.DescriptionBook;
                existingBook.YearPublication = book.YearPublication;
                existingBook.PageCount = book.PageCount;
                existingBook.LanguageBook = book.LanguageBook;
                existingBook.StatusBook = book.StatusBook;

                if (existingBook.IdPublisherNavigation.NamePublisher != book.PublisherName)
                {
                    var publisher = await GetOrCreatePublisherAsync(book.PublisherName);
                    existingBook.IdPublisher = publisher.IdPublisher;
                }

                if (existingBook.IdCategoryNavigation.NameCategory != book.CategoryName)
                {
                    var category = await GetOrCreateCategoryAsync(book.CategoryName);
                    existingBook.IdCategory = category.IdCategory;
                }

                var newAuthors = new List<Author>();
                foreach (var authorDTO in book.Authors)
                {
                    var author = await GetOrCreateAuthorAsync(authorDTO);
                    newAuthors.Add(author);
                }

                UpdateAuthors(existingBook, newAuthors);

                await _context.SaveChangesAsync();

                return Ok("Информация о книге обновлена.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet("admin/authors")]
        public async Task<ActionResult> GetAuthorsAsync()
        {
            try
            {
                var authors = await _context.Authors
                    .Include(a => a.IdBooks)
                    .Select(a => new
                    {
                        IDAuthor = a.IdAuthor,
                        SurnameAuthor = a.SurnameAuthor,
                        NameAuthor = a.NameAuthor,
                        PatronymicAuthor = a.PatronymicAuthor,
                        CountBooks = a.IdBooks.Count,
                        Books = a.IdBooks.Select(b => new
                        {
                            IDBook = b.IdBook,
                            ImageURL = b.ImageUrl,
                            TitleBook = b.TitleBook
                        })
                    })
                    .ToListAsync();

                return Ok(authors);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Администратор")]
        [HttpPut("admin/edit-author/{idAuthor}")]
        public async Task<ActionResult> EditAuthorAsync(int idAuthor, [FromBody] AuthorDTO author)
        {
            try
            {
                var existingAuthor = await _context.Authors
                    .FirstOrDefaultAsync(a => a.IdAuthor == idAuthor);

                existingAuthor.SurnameAuthor = author.SurnameAuthor;
                existingAuthor.NameAuthor = author.NameAuthor;
                existingAuthor.PatronymicAuthor = author.PatronymicAuthor;

                await _context.SaveChangesAsync();

                return Ok("Информация об авторе обновлена.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Администратор")]
        [HttpDelete("admin/delete-author/{idAuthor}")]
        public async Task<ActionResult> DeleteAuthorAsync(int idAuthor)
        {
            try
            {
                var deletingAuthor = await _context.Authors
                    .FirstOrDefaultAsync(a => a.IdAuthor == idAuthor);

                _context.Authors.Remove(deletingAuthor);

                await _context.SaveChangesAsync();

                return Ok("Автор удален.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet("admin/categories")]
        public async Task<ActionResult> GetCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.Books)
                    .Select(c => new
                    {
                        IDCategory = c.IdCategory,
                        NameCategory = c.NameCategory,
                        CountBooks = c.Books.Count,
                        Books = c.Books.Select(b => new
                        {
                            IDBook = b.IdBook,
                            ImageURL = b.ImageUrl,
                            TitleBook = b.TitleBook
                        })
                    })
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Администратор")]
        [HttpPut("admin/edit-category/{idCategory}/{newNameCategory}")]
        public async Task<ActionResult> EditCategoryAsync(int idCategory, string newNameCategory)
        {
            try
            {
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.IdCategory == idCategory);

                existingCategory.NameCategory = newNameCategory;

                await _context.SaveChangesAsync();

                return Ok("Информация о категории обновлена.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Администратор")]
        [HttpDelete("admin/delete-category/{idCategory}")]
        public async Task<ActionResult> DeleteCategoryAsync(int idCategory)
        {
            try
            {
                var deletingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.IdCategory == idCategory);

                _context.Categories.Remove(deletingCategory);

                await _context.SaveChangesAsync();

                return Ok("Категория удалена.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet("admin/staff/{idUser}")]
        public async Task<ActionResult> GetAdminStaffAsync(int idUser)
        {
            try
            {
                var staff = await _context.Users
                    .Where(u => (u.NameRole == "Администратор" || u.NameRole == "Модератор")
                        && u.IdUser != idUser && u.StatusUser == "Активен")
                    .Select(user => new
                    {
                        user.IdUser,
                        user.NameUser,
                        user.LoginUser,
                        user.NameRole
                    })
                    .ToListAsync();

                return Ok(staff);
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost("admin/add-employee")]
        public async Task<ActionResult> AddEmployeeAsync([FromBody] AddEmployeeDTO model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.NameEmployee) || model.NameEmployee.Length > 100)
                {
                    ModelState.AddModelError("NameUser", "Неверный формат имени.");
                }

                if (model.PasswordEmployee != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Пароли не совпадают.");
                }

                if (model.PasswordEmployee.Length < 8)
                {
                    ModelState.AddModelError("ShortPassword", "Пароль должен состоять от 8 символов.");
                }

                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.LoginUser == model.LoginEmployee);
                if (existingUser != null)
                {
                    ModelState.AddModelError("LoginUser", "Данная электронная почта уже занята.");
                }

                if (string.IsNullOrWhiteSpace(model.LoginEmployee))
                {
                    ModelState.AddModelError("LoginUser", "Электронная почта не может быть пустой.");
                }

                if (string.IsNullOrEmpty(model.RoleEmployee))
                {
                    ModelState.AddModelError("RoleUser", "Роль не может быть пустой.");
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                    return BadRequest(new { errors });
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.PasswordEmployee);

                var newEmployee = new User
                {
                    IdUser = _context.Users.Any() ? _context.Users.Max(u => u.IdUser) + 1 : 1,
                    NameRole = model.RoleEmployee,
                    NameUser = model.NameEmployee,
                    LoginUser = model.LoginEmployee,
                    PasswordHash = passwordHash,
                    ProfileImageUrl = null,
                    RegistrationDate = DateOnly.FromDateTime(DateTime.Now),
                    StatusUser = "Активен"
                };
                _context.Users.Add(newEmployee);
                await _context.SaveChangesAsync();

                return Ok("Пользователь добавлен.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Администратор")]
        [HttpPut("admin/change-role/{idUser}/{nameRole}")]
        public async Task<ActionResult> ChangeRoleAsync(int idUser, string nameRole)
        {
            try
            {
                var existingEmployee = await _context.Users
                    .FirstOrDefaultAsync(e => e.IdUser == idUser);

                if (existingEmployee == null) { return NotFound("Пользователь не найден."); }

                existingEmployee.NameRole = nameRole;

                await _context.SaveChangesAsync();

                return Ok("Роль пользователя изменена.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        [Authorize(Roles = "Администратор")]
        [HttpPut("admin/delete-employee/{idUser}")]
        public async Task<ActionResult> DeleteEmployeeAsync(int idUser)
        {
            try
            {
                var existingEmployee = await _context.Users
                    .FirstOrDefaultAsync(e => e.IdUser == idUser);

                if (existingEmployee == null) { return NotFound("Пользователь не найден."); }

                existingEmployee.StatusUser = "Заблокирован";

                await _context.SaveChangesAsync();

                return Ok("Пользователь отстранен.");
            }
            catch (Exception ex) { return HandleException(ex); }
        }

        private static string GetHighQualityImageUrl(string imageUrl, int zoomLevel = 3)
        {
            if (string.IsNullOrEmpty(imageUrl)) return null;

            if (imageUrl.StartsWith("http://"))
                imageUrl = "https://" + imageUrl.Substring(7);

            var uriBuilder = new UriBuilder(imageUrl);
            var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            query["zoom"] = zoomLevel.ToString();
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        private static int? ParseYear(string publishedDate)
        {
            if (string.IsNullOrWhiteSpace(publishedDate))
                return null;

            if (DateTime.TryParseExact(publishedDate, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date.Year;
            }

            if (int.TryParse(publishedDate, out int year) && year > 1000 && year < 3000)
            {
                return year;
            }

            var match = Regex.Match(publishedDate, @"\b(19|20)\d{2}\b");
            if (match.Success && int.TryParse(match.Value, out year))
            {
                return year;
            }

            return null;
        }

        private static readonly Dictionary<string, string> CategoryTranslations = new(StringComparer.OrdinalIgnoreCase)
        {
            // Основные категории
            ["Fiction"] = "Художественная литература",
            ["Fantasy"] = "Фэнтези",
            ["Science Fiction"] = "Научная фантастика",
            ["Computers"] = "Компьютеры",
            ["Cooking"] = "Кулинария",
            ["History"] = "История",
            ["Mathematics"] = "Математика",
            ["Medical"] = "Медицина",
            ["Psychology"] = "Психология",
            ["Science"] = "Наука",
            ["Self-Help"] = "Саморазвитие",
            ["Travel"] = "Путешествия",

            // Дополнения
            ["Art"] = "Искусство",
            ["Religion"] = "Религия",
            ["Sports"] = "Спорт",
            ["Poetry"] = "Поэзия",
            ["Drama"] = "Драматургия",
            ["Juvenile"] = "Детская литература",
            ["Young Adult Fiction"] = "Подростковая литература",

            // Составные категории
            ["Business & Economics"] = "Бизнес и экономика",
            ["Biography & Autobiography"] = "Биографии и мемуары",
            ["Language Arts & Disciplines"] = "Языковые искусства и дисциплины",
            ["Health & Fitness"] = "Здоровье и фитнес",
            ["Technology & Engineering"] = "Технологии и инженерия",

        };

        private List<string> TranslateCategories(List<string> categories)
        {
            return categories.Select(category =>
            {
                if (CategoryTranslations.TryGetValue(category, out var translated))
                    return translated;

                return category;
            }).ToList();
        }

        private async Task<Publisher> GetOrCreatePublisherAsync(string publisherName)
        {
            var publisher = await _context.Publishers
                .FirstOrDefaultAsync(p => p.NamePublisher == publisherName);

            if (publisher == null)
            {
                publisher = new Publisher
                {
                    IdPublisher = _context.Publishers.Any() ? _context.Publishers.Max(p => p.IdPublisher) + 1 : 1,
                    NamePublisher = publisherName
                };
                await _context.Publishers.AddAsync(publisher);
                await _context.SaveChangesAsync();
            }

            return publisher;
        }

        private async Task<Category> GetOrCreateCategoryAsync(string categoryName)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.NameCategory == categoryName);

            if (category == null)
            {
                category = new Category
                {
                    IdCategory = context.Categories.Any() ? _context.Categories.Max(c => c.IdCategory) + 1 : 1,
                    NameCategory = categoryName
                };
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
            }

            return category;
        }

        private async Task<Author> GetOrCreateAuthorAsync(AuthorDTO authorDto)
        {
            var author = await _context.Authors
                .FirstOrDefaultAsync(a =>
                    a.SurnameAuthor == authorDto.SurnameAuthor &&
                    a.NameAuthor == authorDto.NameAuthor &&
                    a.PatronymicAuthor == authorDto.PatronymicAuthor);

            if (author == null)
            {
                author = new Author
                {
                    IdAuthor = _context.Authors.Any() ? _context.Authors.Max(a => a.IdAuthor) + 1 : 1,
                    SurnameAuthor = authorDto.SurnameAuthor,
                    NameAuthor = authorDto.NameAuthor,
                    PatronymicAuthor = authorDto.PatronymicAuthor
                };
                await _context.Authors.AddAsync(author);
                await _context.SaveChangesAsync();
            }

            return author;
        }

        private void UpdateAuthors(Book book, List<Author> newAuthors)
        {
            var authorsToRemove = book.IdAuthors
                .Where(a => !newAuthors.Any(na => na.IdAuthor == a.IdAuthor))
                .ToList();

            foreach (var author in authorsToRemove)
            {
                book.IdAuthors.Remove(author);
            }

            var authorsToAdd = newAuthors
                .Where(na => !book.IdAuthors.Any(a => a.IdAuthor == na.IdAuthor))
                .ToList();

            foreach (var author in authorsToAdd)
            {
                book.IdAuthors.Add(author);
            }
        }

    }
}
