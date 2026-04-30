namespace FrogLib.Server.DTOs
{
    public class BookSearchResult
    {
        public string Title { get; set; }
        public List<string> Authors { get; set; } = new();
        public string Publisher { get; set; }
        public int? YearPublication { get; set; }
        public string Description { get; set; }
        public string Isbn13 { get; set; }
        public int? PageCount { get; set; }
        public string Language { get; set; }
        public List<string> Categories { get; set; } = new();
        public string ImageUrl { get; set; }
    }
}
