using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DTOs;
using OnlineLibrary.Models;

namespace OnlineLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoansController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Loans
        [HttpGet("View-All-Loans")]
        public async Task<ActionResult<IEnumerable<LoanDTO>>> GetLoans()
        {
            return await _context.Loans
                .Include(x => x.LoanCard)
                .ThenInclude(x => x.Customer)
                .Select(l => l.ToLoanDTO()).AsNoTracking().ToListAsync();
        }

        [HttpGet("View-Current-Loans")]
        public async Task<ActionResult<IEnumerable<LoanDTO>>> GetCurrentLoans()
        {
            var loans = await _context.Loans.Where(x => x.BookCopy.IsRented == true)
                .Include(x => x.LoanCard)
                .ThenInclude(x => x.Customer)
                .Select(l => l.ToLoanDTO()).AsNoTracking().ToListAsync();
                
            return loans;
        }

        [HttpGet("View-Previous-Loans")]
        public async Task<ActionResult<IEnumerable<LoanDTO>>> GetPreviousLoans()
        {
            var loans = await _context.Loans.Where(x => x.BookCopy.IsRented == false)
                .Include(x => x.LoanCard)
                .ThenInclude(x => x.Customer)
                .Select(l => l.ToLoanDTO()).AsNoTracking().ToListAsync();
            
            return loans;
        }

        // GET: api/Loans/5
        [HttpGet("Search-Loan-By-Id")]
        public async Task<ActionResult<LoanDTO>> GetLoan(int id)
        {
            var loan= await _context.Loans
                .Include(x => x.LoanCard)
                .ThenInclude(x => x.Customer)
                .Where(x => x.Id.Equals(id))
                .FirstOrDefaultAsync();

            if (loan == null)
            {
                return NotFound("Could not find a loan with that id");
            }

            return loan.ToLoanDTO();
        } 

        [HttpPut("Return-Book-By-Loan-Id")]
        public async Task<ActionResult<LoanDTO>> ReturnBook(int id)
        {
            var loan = await _context.Loans
                .Include(x => x.LoanCard)
                .ThenInclude(x => x.Customer)
                .Where(x => x.Id.Equals(id))
                .FirstOrDefaultAsync();

            if (loan == null)
            {
                return NotFound("Could not find loan");
            }

            loan.ReturnDate = DateTime.Now;
            var book = await _context.BookCopies.FirstOrDefaultAsync(x => x.Id.Equals(loan.BookId));
            
            if (book == null)
            {
                return NotFound("Could not find a book tied to the loan");
            }

            book.IsRented = false;

            try
            {
                await _context.SaveChangesAsync();
                
                return loan.ToLoanDTO();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Loans
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Rent-Book")]
        public async Task<ActionResult<LoanDTO>> PostLoan(LoanCreateDTO loanCreateDTO)
        {
            var book = await _context.BookCopies.FirstOrDefaultAsync(x => x.Id.Equals(loanCreateDTO.BookCopyId));
            
            if (book == null)
            {
                return NotFound("Could not find the book to rent");
            }

            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.LoanCardId.Equals(loanCreateDTO.LoanCardId));

            if (customer == null)
            {
                return NotFound("Could not rent book, no customer is tied to that loan card");
            }

            if (book.IsRented == true)
            {
                return NotFound("Could not rent book, already rented");
            }

            var customerAlreadyRented = await _context.Loans.FirstOrDefaultAsync(x => x.LoanCardId.Equals(customer.LoanCardId) && x.BookCopy.ISBNId.Equals(book.ISBNId) && x.BookCopy.IsRented == true);
            
            if (customerAlreadyRented != null)
            {
                return NotFound("Customer is currently renting a book with the same ISBN. Can't rent two identical books at the same time");
            }

            book.IsRented = true;
            var loan = loanCreateDTO.ToLoan(_context);

            loan.LoanDate = DateTime.Today;
            loan.ReturnDate = DateTime.Today.AddDays(30);

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan.ToLoanDTO());
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.Id == id);
        } 
    }
}
