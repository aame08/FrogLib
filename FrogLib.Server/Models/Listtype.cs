namespace FrogLib.Server.Models;

public partial class Listtype
{
    public int IdListType { get; set; }

    public string NameList { get; set; } = null!;

    public virtual ICollection<Userbook> Userbooks { get; set; } = new List<Userbook>();
}
