namespace OnlineLibrary.Models;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int LoanCardId { get; set; }
    public LoanCard LoanCard { get; set; }
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}