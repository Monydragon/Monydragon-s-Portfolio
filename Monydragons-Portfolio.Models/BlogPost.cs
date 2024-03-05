namespace Monydragon_Portfolios.Models;

public class BlogPost
{
    public DateTime Date { get; set; }
    public string Title { get; set; }
    public List<string> ContentFiles { get; set; }
    public List<string> ImageFiles { get; set; }
}
