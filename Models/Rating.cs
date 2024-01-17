using Microsoft.AspNetCore.Routing.Constraints;

namespace OnlineLibrary.Models;
public class Rating
{
    public int Id { get; set; }
    public int BookRating { get; set; }
    public string? Review { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    public int ISBNId { get; set; }
    public ISBN ISBN { get; set; }
}
