namespace OnlineLibrary.Models;

public class BookCopy
{
    public int Id { get; set; }
    public bool IsRented { get; set; }
    public int ISBNId { get; set; }
    public ISBN ISBN { get; set; }
    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
