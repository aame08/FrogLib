namespace FrogLib.Server.DTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Author { get; set; }
        public string AuthorURL { get; set; }
        public DateOnly Date { get; set; }
        public string Content { get; set; }
        public bool IsReply { get; set; }
        public List<CommentDTO> Replies { get; set; }
    }
}
