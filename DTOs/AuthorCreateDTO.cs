namespace OnlineLibrary.DTOs;

public class AuthorCreateDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<int> ISBNIds { get; set; } = new();
}
