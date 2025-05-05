namespace FrogLib.Server.Models;

public partial class Comment
{
    public int IdComment { get; set; }

    public int IdUser { get; set; }

    public int IdEntity { get; set; }

    public string TypeEntity { get; set; } = null!;

    public string TextComment { get; set; } = null!;

    public DateOnly DateComment { get; set; }

    public int? ParentCommentId { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    public virtual Comment? ParentComment { get; set; }
}
