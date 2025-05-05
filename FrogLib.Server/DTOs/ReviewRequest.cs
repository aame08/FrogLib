namespace FrogLib.Server.DTOs
{
    public class ReviewRequest
    {
        public int IdUser { get; set; }
        public int IdBook { get; set; }
        public string TitleReview { get; set; }
        public string TextReview { get; set; }
        public int NewRating { get; set; }
    }
}
