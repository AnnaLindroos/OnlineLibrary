namespace OnlineLibrary.DTOs;

public class BookCopyDTO
{
    public int Id { get; set; }
    public bool IsRented { get; set; }
    public int ISBNId { get; set; }
    public string Title { get; set; }
}
