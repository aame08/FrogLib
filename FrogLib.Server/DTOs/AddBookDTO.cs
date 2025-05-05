namespace FrogLib.Server.DTOs
{
    public class AddBookDTO
    {
        public string Isbn10 { get; set; }
        public string Isbn13 { get; set; }
        public string PublisherName { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public string TitleBook { get; set; }
        public string DescriptionBook { get; set; }
        public short YearPublication { get; set; }
        public int PageCount { get; set; }
        public string LanguageBook { get; set; }
        public string? StatusBook { get; set; }
        public List<AuthorDTO> Authors { get; set; } = new();
    }

    public class AuthorDTO
    {
        public string SurnameAuthor { get; set; } = null!;

        public string NameAuthor { get; set; } = null!;

        public string? PatronymicAuthor { get; set; }
    }
}
