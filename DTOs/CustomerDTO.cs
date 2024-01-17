namespace OnlineLibrary.DTOs;

public class CustomerDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int LoanCardId { get; set; }
    public List<int> RatingIds { get; set; } = new();
}
