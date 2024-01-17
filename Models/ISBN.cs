namespace OnlineLibrary.Models;

public class ISBN
{
    public int Id { get; set; }
    public long ISBNNumber { get; set; }
    public int ReleaseYear { get; set; }
    public string Title { get; set; }
    public List<Author> Authors { get; set; } = new();
    public List<Rating> Ratings { get; set; } = new();
    public List<BookCopy> BookCopies { get; set; } = new();
}