namespace OnlineLibrary.DTOs;

public class ISBNDTO
{
    public int Id { get; set; }
    public long ISBNNumber { get; set; }
    public int ReleaseYear { get; set; }
    public string Title { get; set; }
    public int AvailableCopies { get; set; } 
    public List<string> Authors { get; set; } = new();
}
