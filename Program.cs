using Microsoft.CodeAnalysis.Elfie.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DTOs;
using OnlineLibrary.Models;
using System.Linq;
using System.Xml.Xsl;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => 
{
    var connectionString = builder.Configuration.GetConnectionString("OnlineLibraryDb");
    var connBuilder = new SqlConnectionStringBuilder(connectionString);
    /*{
        Password = builder.Configuration["DbPassword"]
    }; */
    connectionString = connBuilder.ConnectionString;
    opt.UseSqlServer(connectionString);
});


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
    AddTestData(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void AddTestData(AppDbContext db)
{
    // Add ISBNS
    db.Add(new ISBN { ISBNNumber = 9798871174845, ReleaseYear = 2023, Title = "Lethal Company Game Guide: Discover the Ultimate Strategies for Survival and Unlock the Secrets of the Indie Sensation with Secret Tips, Tricks, and In-Depth Guides for Beginners to Pros" });
    db.Add(new ISBN { ISBNNumber = 9780593234068, ReleaseYear = 2022, Title = "How to Be More Shrek " });
    db.Add(new ISBN { ISBNNumber = 9780593234044, ReleaseYear = 2022, Title = "Everything I Need to Know I Learned from E.T. the Extra-Terrestrial" });
    db.Add(new ISBN { ISBNNumber = 9780099579939, ReleaseYear = 2012, Title = "Fifty Shades of Grey" });
    db.Add(new ISBN { ISBNNumber = 9781616550417, ReleaseYear = 2013, Title = "The Legend of Zelda" });
    db.Add(new ISBN { ISBNNumber = 9781088161753, ReleaseYear = 2023, Title = "Somehow, I Manage" });
    db.Add(new ISBN { ISBNNumber = 9780593712849, ReleaseYear = 2023, Title = "Choose Your Enemies Wisely" });
    db.SaveChanges();

    // Add Authors
    db.Add(new Author { FirstName = "George", LastName = "Hannon", ISBNs = db.ISBNs.Where(x => x.Id.Equals(1)).ToList() });
    db.Add(new Author { FirstName = "NBC", LastName = "Universal", ISBNs = db.ISBNs.Where(x => x.Id >= 2 && x.Id <= 3).ToList() });
    db.Add(new Author { FirstName = "EL", LastName = "James", ISBNs = db.ISBNs.Where(x => x.Id.Equals(4)).ToList() });
    db.Add(new Author { FirstName = "Shigeru", LastName = "Miyamoto", ISBNs = db.ISBNs.Where(x => x.Id.Equals(5)).ToList()});
    db.Add(new Author { FirstName = "Michael", LastName = "Scott", ISBNs = db.ISBNs.Where(x => x.Id.Equals(6)).ToList() });
    db.Add(new Author { FirstName = "Patrick", LastName = "Bet-David", ISBNs = db.ISBNs.Where(x => x.Id.Equals(7)).ToList() });
    db.Add(new Author { FirstName = "Greg", LastName = "Dinkin", ISBNs = db.ISBNs.Where(x => x.Id.Equals(7)).ToList() });
    db.SaveChanges();

    // Add Book copies
    db.Add(new BookCopy { IsRented = false, ISBNId = 1, ISBN = db.ISBNs.FirstOrDefault(x => x.Id.Equals(1)) });
    db.Add(new BookCopy { IsRented = false, ISBNId = 2, ISBN = db.ISBNs.FirstOrDefault(x => x.Id.Equals(2)) });
    db.Add(new BookCopy { IsRented = false, ISBNId = 3, ISBN = db.ISBNs.FirstOrDefault(x => x.Id.Equals(3)) });
    db.Add(new BookCopy { IsRented = false, ISBNId = 4, ISBN = db.ISBNs.FirstOrDefault(x => x.Id.Equals(4)) });
    db.Add(new BookCopy { IsRented = false, ISBNId = 5, ISBN = db.ISBNs.FirstOrDefault(x => x.Id.Equals(5)) });
    db.Add(new BookCopy { IsRented = false, ISBNId = 6, ISBN = db.ISBNs.FirstOrDefault(x => x.Id.Equals(6)) });
    db.Add(new BookCopy { IsRented = false, ISBNId = 7, ISBN = db.ISBNs.FirstOrDefault(x => x.Id.Equals(7)) });
    db.SaveChanges();

    // Add a Loan Card
    db.Add(new LoanCard { LoanCardNumber = 1001 });
    db.SaveChanges();

    // Add Customer
    db.Add(new Customer { FirstName = "Klonken", LastName = "Svensson", LoanCardId = 1, LoanCard = db.LoanCards.FirstOrDefault(x => x.Id.Equals(1)) });
    db.SaveChanges();

    // Rent a book
    var currentDate = DateTime.Today;
    var loan = new Loan
    {
        LoanDate = currentDate,
        ReturnDate = currentDate.AddDays(30),
        BookId = 1,
        BookCopy = db.BookCopies.FirstOrDefault(x => x.Id.Equals(1)),
        LoanCardId = 1,
        LoanCard = db.LoanCards.FirstOrDefault(x => x.Id.Equals(1))
    };
    db.Add(loan);
    loan.BookCopy.IsRented = true;
    db.SaveChanges();

    // Add a rating
    db.Add(new Rating
    {
        BookRating = 5,
        Review = "Very good book I'm slaying at the game now",
        CustomerId = 1,
        Customer = db.Customers.FirstOrDefault(x => x.Id.Equals(1)),
        ISBNId = 1,
        ISBN = db.ISBNs.FirstOrDefault(x => x.Id.Equals(1))
    });
    db.SaveChanges();

    // Return book
    var loanToReturn = db.Loans.FirstOrDefault(x => x.Id.Equals(1));
    loanToReturn.ReturnDate = DateTime.Now;
    loanToReturn.BookCopy.IsRented = false;
    db.SaveChanges();
}
