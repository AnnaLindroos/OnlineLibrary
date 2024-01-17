namespace OnlineLibrary.DTOs;

public class AuthorDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<int> ISBNIds { get; set; } 
}
