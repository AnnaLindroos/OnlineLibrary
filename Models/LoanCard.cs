namespace OnlineLibrary.Models;

public class LoanCard
{
    public int Id { get; set; }
    public int LoanCardNumber { get; set; }
    public Customer? Customer { get; set; }
}