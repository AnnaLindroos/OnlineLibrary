using OnlineLibrary.Models;

namespace OnlineLibrary.DTOs;

public class LoanDTO
{
    public int Id { get; set; }
    public string LoanDate { get; set; } 
    public string ReturnDate { get; set; }
    public string CustomerName { get; set; }
    public int BookId { get; set; }
    public int LoanCardId { get; set; }
}