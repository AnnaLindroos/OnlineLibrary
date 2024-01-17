using OnlineLibrary.Models;

namespace OnlineLibrary.DTOs;

public class RatingDTO
{
    public int Id { get; set; }
    public string BookTitle { get; set; }
    public int BookRating { get; set; }
    public string? Review { get; set; }
    public string CustomerName { get; set; }
    public int ISBNId { get; set; }
    public int CustomerId { get; set; }
}
