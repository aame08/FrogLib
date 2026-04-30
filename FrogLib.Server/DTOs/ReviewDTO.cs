namespace FrogLib.Server.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageURL { get; set; }
        public double Rating { get; set; }
        public int CountView { get; set; }
        public DateOnly CreatedDate { get; set; }
        public string UserName { get; set; }
        public string UserURL { get; set; }
        public List<CommentDTO> Comments { get; set; }
    }
}
