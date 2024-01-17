namespace OnlineLibrary.DTOs;

public class RatingCreateDTO
{
    public int BookRating { get; set; }
    public string? Review { get; set; }
    public int CustomerId { get; set; }
    public int ISBNId { get; set; }
}
