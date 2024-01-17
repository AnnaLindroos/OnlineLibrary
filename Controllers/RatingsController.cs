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
    public class RatingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RatingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Ratings
        [HttpGet("View-All-Ratings")]
        public async Task<ActionResult<IEnumerable<RatingDTO>>> GetRatings()
        {
            return await _context.Ratings
                .Include(x => x.ISBN)
                .Include(x => x.Customer)
                .Select(r => r.ToRatingDTO()).AsNoTracking().ToListAsync();
        }

        // GET: api/Ratings/5
        [HttpGet("Search-Rating-By-Id")]
        public async Task<ActionResult<RatingDTO>> GetRating(int id)
        {
            var rating = await _context.Ratings
                .Include(x => x.ISBN)
                .Include(x => x.Customer)
                .Where(x => x.Id.Equals(id))
                .FirstOrDefaultAsync();

            if (rating == null)
            {
                return NotFound("Could not find a rating with that id");
            }

            return rating.ToRatingDTO();
        }

        // PUT: api/Ratings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        
        [HttpPut("Edit-Rating-By-Id")]
        public async Task<IActionResult> PutRating(int id, RatingDTO ratingDTO)
        {
            if (id != ratingDTO.Id)
            {
                return BadRequest();
            }

            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound("Could not find a rating to edit with that id");
            }

            var customer = ratingDTO.CustomerId;
            var isbn = ratingDTO.ISBNId;

            rating.BookRating = ratingDTO.BookRating;
            rating.Review = ratingDTO.Review;
            rating.CustomerId = ratingDTO.CustomerId;
            rating.Customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id.Equals(customer));

            if (rating.Customer == null)
            {
                return NotFound("Could not find the customer tied to the rating");
            }
            rating.ISBNId = ratingDTO.ISBNId;
            rating.ISBN = await _context.ISBNs.FirstOrDefaultAsync(x => x.Id.Equals(isbn));

            if (rating.ISBN == null)
            {
                return NotFound("Could not find the ISBN tied to the rating");
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(id))
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

        // POST: api/Ratings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Add-Rating")]
        public async Task<ActionResult<RatingDTO>> PostRating(RatingCreateDTO ratingCreateDTO)
        {
            var rating = ratingCreateDTO.ToRating(_context);

            if (rating.BookRating <= 0 || rating.BookRating > 5)
            {
                return NotFound("Please rate the book from 1 to 5, 5 being the highest rating");
            }

            var customerAlreadyRated = await _context.Ratings.FirstOrDefaultAsync(x => x.ISBNId.Equals(rating.ISBNId) && x.CustomerId.Equals(rating.CustomerId));

            if (customerAlreadyRated != null)
            {
                return NotFound("The customer has already rated this book. Please edit the existing rating instead.");
            }

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRating), new { id = rating.Id }, rating.ToRatingDTO());
        }

        // DELETE: api/Ratings/5
        [HttpDelete("Delete-Rating-By-Id")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound("Could not find a rating with that id to delete");
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return Ok("Rating deleted successfully");
        }

        private bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.Id == id);
        }
    }
}
