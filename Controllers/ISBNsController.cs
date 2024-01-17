using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using OnlineLibrary.DTOs;
using OnlineLibrary.Migrations;
using OnlineLibrary.Models;

namespace OnlineLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ISBNsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ISBNsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ISBNs
        [HttpGet("View-Information-All-Books")]
        public async Task<ActionResult<IEnumerable<ISBNDTO>>> GetISBNs()
        {
            var isbns = await _context.ISBNs
                .Include(x => x.Authors)
                .Include(x => x.BookCopies)
                .Select(i => i.ToISBNDTO()).AsNoTracking().ToListAsync();

            return isbns;
        }

        // GET: api/ISBNs/5
        [HttpGet("Search-Book-Information-By-Id")]
        public async Task<ActionResult<ISBNDTO>> GetISBN(int id)
        {
            var iSBN = await _context.ISBNs
                .Include(x => x.Authors)
                .Include(x => x.BookCopies)
                .Where(x => x.Id.Equals(id))
                .FirstOrDefaultAsync();

            if (iSBN == null)
            {
                return NotFound("Could not find an ISBN with that id");
            }

            return iSBN.ToISBNDTO();
        }

        // POST: api/ISBNs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Add-ISBN")]
        public async Task<ActionResult<ISBNDTO>> PostISBN(ISBNCreateDTO iSBNCreateDTO)
        {
            if (iSBNCreateDTO.ISBNNumber.ToString().Length != 13)
            {
                return NotFound("Length of ISBN must be 13 numbers");
            }

            if (iSBNCreateDTO.ReleaseYear < 0)
            {
                return NotFound("Release year cannot be a negative number");
            }

            var isbnExists = await _context.ISBNs.FirstOrDefaultAsync(x => x.ISBNNumber.Equals(iSBNCreateDTO.ISBNNumber));

            if (isbnExists != null)
            {
                return NotFound("An ISBN with that ISBNNumber already exists");
            }

            var iSBN = iSBNCreateDTO.ToISBN(_context);
            _context.ISBNs.Add(iSBN);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetISBN), new { id = iSBN.Id }, iSBN.ToISBNDTO());
        }

        // DELETE: api/ISBNs/5
        [HttpDelete("Delete-ISBN-By-Id")]
        public async Task<IActionResult> DeleteISBN(int id)
        {
            var iSBN = await _context.ISBNs.FindAsync(id);
            if (iSBN == null)
            {
                return NotFound("Could not find an ISBN with that id to delete");
            }

            var books = _context.BookCopies.Where(x => x.ISBN.Equals(iSBN));

            if (books.Any(x => x.IsRented == true))
            {
                return NotFound("One or more books with that ISBN are rented. Wait for books to get back in stock before deleting");
            }

            _context.ISBNs.Remove(iSBN);
            await _context.SaveChangesAsync();

            return Ok("ISBN deleted successfully");
        }

        private bool ISBNExists(int id)
        {
            return _context.ISBNs.Any(e => e.Id == id);
        }
    }
}
