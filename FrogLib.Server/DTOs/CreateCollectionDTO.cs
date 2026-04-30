namespace FrogLib.Server.DTOs
{
    public class CreateCollectionDTO
    {
        public int IdUser { get; set; }
        public string TitleCollection { get; set; } = null!;
        public string? DescriptionCollection { get; set; }
        public List<int> IdBooks { get; set; } = new List<int>();
    }
}
