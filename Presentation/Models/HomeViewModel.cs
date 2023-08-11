namespace Presentation.Models;

public class HomeViewModel
{
    public int UsersCount { get; set; }
    public int ViewCountStep { get; init; }

    public HomeViewModel()
    {
        ViewCount = ViewCountStep = 10;
        Users = new();
    }

    public int ViewCount { get; set; }
    public List<UserViewModel> Users { get; set; }
}
