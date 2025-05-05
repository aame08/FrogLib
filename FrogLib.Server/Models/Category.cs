namespace FrogLib.Server.Models;

public partial class Category
{
    public int IdCategory { get; set; }

    public string NameCategory { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
