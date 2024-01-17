using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using OnlineLibrary.DTOs;
using OnlineLibrary.Models;

namespace OnlineLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthorsController(AppDbContext context)
        {

            _context = context;
        }

        [HttpGet("View-All-Authors")]
        public async Task<ActionResult<IEnumerable<AuthorDTO>>> GetAuthors()
        {
            return await _context.Authors
                .Include(x => x.ISBNs)
                .Select(m => m.ToAuthorDTO()).AsNoTracking().ToListAsync();
        }

        // GET: api/Authors/5
        [HttpGet("Search-Author-By-Id")]
        public async Task<ActionResult<AuthorDTO>> GetAuthor(int id)
        {
            var author = await _context.Authors
                .Include(x => x.ISBNs)
                .Where(x => x.Id.Equals(id))
                .FirstOrDefaultAsync();

                if (author == null)
                {
                    return NotFound("Could not find an author with that id");
                }

                return author.ToAuthorDTO();
        } 


        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Add-Isbn-By-Author-Id")]
        public async Task<IActionResult> PutAuthor(int authorId, int isbnId)
        {
            var author = await _context.Authors.FindAsync(authorId);

            if (author == null)
            {
                return NotFound("Could not find an author to edit with that id");
            }

            var isbnToAdd = await _context.ISBNs.FirstOrDefaultAsync(x => x.Id.Equals(isbnId));

            if (isbnToAdd == null)
            {
                return NotFound("Could not find an ISBN with that id to add");
            }

            author.ISBNs.Add(isbnToAdd);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!AuthorExists(authorId))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("Edit-Author-By-Id")]
        public async Task<IActionResult> PutAuthor(int id, AuthorDTO authorDTO)
        {
            if (id != authorDTO.Id)
            {
                return BadRequest();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound("Could not find an author with that id");
            }

            var isbnIds = authorDTO.ISBNIds.ToList();

            author.FirstName = authorDTO.FirstName;
            author.LastName = authorDTO.LastName;
            author.ISBNs = _context.ISBNs.Where(x => isbnIds.Contains(x.Id)).ToList(); ;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Add-Author")]
        public async Task<ActionResult<AuthorDTO>> PostAuthor(AuthorCreateDTO authorCreateDTO)
        {
            var author = authorCreateDTO.ToAuthor(_context);
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author.ToAuthorDTO());
        }

        // DELETE: api/Authors/5
        [HttpDelete("Delete-Author-By-Id")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound("Could not find author to delete");
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return Ok("Author deleted succesfully");
        }

        private bool AuthorExists(long id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
