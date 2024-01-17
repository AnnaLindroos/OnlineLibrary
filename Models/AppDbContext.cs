using Microsoft.EntityFrameworkCore;
using System.Diagnostics;


namespace OnlineLibrary.Models;

public class AppDbContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<BookCopy> BookCopies { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<ISBN> ISBNs { get; set; }
    public DbSet<LoanCard> LoanCards { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasOne<LoanCard>(l => l.LoanCard)
            .WithOne(c => c.Customer);

        modelBuilder.Entity<LoanCard>()
            .HasOne<Customer>(c => c.Customer)
            .WithOne(l => l.LoanCard)
            .HasForeignKey<Customer>(l => l.LoanCardId); 

        modelBuilder.Entity<LoanCard>()
            .HasIndex(l => l.LoanCardNumber)
            .IsUnique();

        modelBuilder.Entity<ISBN>()
            .HasIndex(i => i.ISBNNumber)
            .IsUnique();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.LogTo(message => Debug.WriteLine(message));
        options.EnableSensitiveDataLogging();
    }
}