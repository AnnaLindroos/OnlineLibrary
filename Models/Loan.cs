namespace OnlineLibrary.Models;

public class Loan
{
    public int Id { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime ReturnDate { get; set; }
    public int BookId { get; set; }
    public BookCopy BookCopy { get; set; }
    public int LoanCardId { get; set; }
    public LoanCard LoanCard { get; set; }
}