using System;
using System.Collections.Generic;
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
    public class BookCopiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookCopiesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/BookCopies
        [HttpGet("View-All-Book-Copies")]
        public async Task<ActionResult<IEnumerable<BookCopyDTO>>> GetBooks()
        {
            return await _context.BookCopies
                .Include(x => x.ISBN)
                .Include(x => x.Loans)
                .Select(b => b.ToBookCopyDTO())
                .ToListAsync();
        }

        [HttpGet("View-All-Book-Copies-Available-For-Rent")]
        public async Task<ActionResult<IEnumerable<BookCopyDTO>>> GetAvailableBooks()
        {
            return await _context.BookCopies
                .Include(x => x.ISBN)
                .Include(x => x.Loans)
                .Where(x => x.IsRented == false)
                .Select(b => b.ToBookCopyDTO())
                .ToListAsync();
        }

        // GET: api/BookCopies/5
        [HttpGet("Search-Book-Copy-By-Id")]
        public async Task<ActionResult<BookCopyDTO>> GetBook(int id)
        {
            var book = await _context.BookCopies
                .Include(x => x.ISBN)
                .Include(x => x.Loans)
                .Where(x => x.Id.Equals(id))
                .Select(b => b.ToBookCopyDTO())
                .FirstOrDefaultAsync();

                if (book == null)
                {
                    return NotFound("Could not find a book with that id");
                }

                return book;
        }

        // POST: api/BookCopies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Add-Book-Copy")]
        public async Task<ActionResult<BookCopyDTO>> PostBook(BookCopyCreateDTO bookCreateDTO)
        {
            var book = bookCreateDTO.ToBookCopy(_context);
            _context.BookCopies.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book.ToBookCopyDTO());
        }

        // DELETE: api/BookCopies/5
        [HttpDelete("Delete-Book-Copy-By-Id")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.BookCopies.FindAsync(id);
            if (book == null)
            {
                return NotFound("Could not find a book with that id to delete");
            }
            if (book.IsRented == true)
            {
                return NotFound("Book is rented. Please wait for book to be returned before deleting it");
            }

            _context.BookCopies.Remove(book);
            await _context.SaveChangesAsync();

            return Ok("Book copy deleted successfully");
        }

        private bool BookExists(int id)
        {
            return _context.BookCopies.Any(e => e.Id == id);
        }
    }
}
