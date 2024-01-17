using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using NuGet.Packaging.Signing;
using OnlineLibrary.Controllers;
using OnlineLibrary.DTOs;
using System.Composition;
using System.Reflection;

namespace OnlineLibrary.Models;

public static class ModelExtensions
{
    public static Author ToAuthor(this AuthorCreateDTO authorCreateDTO, AppDbContext context)
    {
        var isbnIds = authorCreateDTO.ISBNIds.ToList();
        return new Author
        {
            FirstName = authorCreateDTO.FirstName,
            LastName = authorCreateDTO.LastName,
            ISBNs = context.ISBNs.Where(x => isbnIds.Contains(x.Id)).ToList()
        };
    }

    public static AuthorDTO ToAuthorDTO(this Author author)
    {
        return new AuthorDTO
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName,
            ISBNIds = author.ISBNs.Select(x => x.Id).ToList()
        };
    }

    public static BookCopy ToBookCopy(this BookCopyCreateDTO bookCreateDTO, AppDbContext context)
    {
        var isbn = context.ISBNs.FirstOrDefault(x => x.Id.Equals(bookCreateDTO.ISBNId));

        if (isbn == null)
        {
            throw new NullReferenceException("Could not find the ISBN");
        }

        return new BookCopy
        {
            IsRented = false,
            ISBNId = bookCreateDTO.ISBNId,
            ISBN = isbn
        };
    }

    public static BookCopyDTO ToBookCopyDTO(this BookCopy book)
    {
        return new BookCopyDTO
        {
            Id = book.Id,
            IsRented = book.IsRented,
            ISBNId = book.ISBNId,
            Title = book.ISBN.Title,
        };
    }

    public static Customer ToCustomer(this CustomerCreateDTO customerCreateDTO, AppDbContext context)
    {
        return new Customer
        {
            FirstName = customerCreateDTO.FirstName,
            LastName = customerCreateDTO.LastName,
        };
    }

    public static CustomerDTO ToCustomerDTO(this Customer customer)
    {
        return new CustomerDTO
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            LoanCardId = customer.LoanCardId,
            RatingIds = customer.Ratings.Select(x => x.Id).ToList()
        };
    }

    public static ISBN ToISBN(this ISBNCreateDTO iSBNCreateDTO, AppDbContext context)
    {
        var books = context.BookCopies.Where(x => x.ISBN.ISBNNumber.Equals(iSBNCreateDTO.ISBNNumber));

        return new ISBN
        {
            ISBNNumber = iSBNCreateDTO.ISBNNumber,
            ReleaseYear = iSBNCreateDTO.ReleaseYear,
            Title = iSBNCreateDTO.Title,
        };
    }

    public static ISBNDTO ToISBNDTO(this ISBN isbn)
    {
        return new ISBNDTO
        {
            Id = isbn.Id,
            ISBNNumber = isbn.ISBNNumber,
            ReleaseYear = isbn.ReleaseYear,
            Title = isbn.Title,
            Authors = isbn.Authors.Select(x => $"{x.FirstName} {x.LastName}").ToList(),
            AvailableCopies = isbn.BookCopies.Where(x => x.IsRented.Equals(false)).Count()
        };
    }

    public static LoanCard ToLoanCard(this LoanCardCreateDTO loanCardCreateDTO, AppDbContext context)
    {

        int newNumber = context.LoanCards.Count() == 0 ? newNumber = 1000 : newNumber = context.LoanCards.Max(x => x.LoanCardNumber) + 1;

        return new LoanCard
        {
            LoanCardNumber = newNumber
        };
    }

    public static LoanCardDTO ToLoanCardDTO(this LoanCard loanCard)
    {
        return new LoanCardDTO
        {
            Id = loanCard.Id,
            LoanCardNumber = loanCard.LoanCardNumber,
        };
    }

    public static Loan ToLoan(this LoanCreateDTO loanCreateDTO, AppDbContext context)
    {
        var book = context.BookCopies.FirstOrDefault(x => x.Id.Equals(loanCreateDTO.BookCopyId));
        if (book == null)
        {
            throw new NullReferenceException("Couldn't find the book copy");
        }

        var loanCard = context.LoanCards.FirstOrDefault(x => x.Id.Equals(loanCreateDTO.LoanCardId));
        if (loanCard == null)
        {
            throw new NullReferenceException("Couldn't find the loan card");
        }

        return new Loan
        {
            LoanDate = DateTime.Today,
            ReturnDate = DateTime.Today.AddDays(30),
            BookId = loanCreateDTO.BookCopyId,
            BookCopy = book,
            LoanCardId = loanCreateDTO.LoanCardId,
            LoanCard = loanCard
        };
    }

    public static LoanDTO ToLoanDTO(this Loan loan)
    {
        return new LoanDTO
        {
            Id = loan.Id,
            LoanDate = loan.LoanDate.ToShortDateString(),
            ReturnDate = loan.ReturnDate.ToShortDateString(),
            CustomerName = $"{loan.LoanCard.Customer.FirstName} {loan.LoanCard.Customer.LastName}",
            BookId = loan.BookId,
            LoanCardId = loan.LoanCardId,
        };
    }

    public static Rating ToRating(this RatingCreateDTO ratingCreateDTO, AppDbContext context)
    {
        var customer = context.Customers.FirstOrDefault(x => x.Id.Equals(ratingCreateDTO.CustomerId));
        if (customer == null)
        {
            throw new NullReferenceException("Couldn't find the customer");
        }

        var isbn = context.ISBNs.FirstOrDefault(x => x.Id.Equals(ratingCreateDTO.ISBNId));
        if (isbn == null)
        {
            throw new NullReferenceException("Couldn't find the isbn");
        }

        return new Rating
        {
            BookRating = ratingCreateDTO.BookRating,
            Review = ratingCreateDTO.Review,
            CustomerId = ratingCreateDTO.CustomerId,
            Customer = customer,
            ISBNId = ratingCreateDTO.ISBNId,
            ISBN = isbn
        };
    }

    public static RatingDTO ToRatingDTO(this Rating rating)
    {
        string review;
        if (rating.Review == "string")
        {
            review = string.Empty;
        }
        else
        {
            review = rating.Review;
        }

        return new RatingDTO
        {
            Id = rating.Id,
            BookTitle = rating.ISBN.Title,
            BookRating = rating.BookRating,
            Review = review,
            CustomerId = rating.CustomerId,
            CustomerName = $"{rating.Customer.FirstName} {rating.Customer.LastName}",
            ISBNId = rating.ISBNId
        };
    }
}
