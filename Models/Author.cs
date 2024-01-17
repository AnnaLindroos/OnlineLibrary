using System.Security.Cryptography.X509Certificates;

namespace OnlineLibrary.Models;

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<ISBN> ISBNs { get; set; } = new List<ISBN>();
}
