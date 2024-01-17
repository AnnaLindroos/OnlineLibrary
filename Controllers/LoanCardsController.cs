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
    public class LoanCardsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoanCardsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/LoanCard
        [HttpGet("View-All-Loan-Cards")]
        public async Task<ActionResult<IEnumerable<LoanCardDTO>>> GetLoanCards()
        {
            return await _context.LoanCards.Select(m => m.ToLoanCardDTO()).AsNoTracking().ToListAsync();
        }

        // GET: api/LoanCard/5
        [HttpGet("Search-Loan-Card-By-Id")]
        public async Task<ActionResult<LoanCardDTO>> GetLoanCard(int id)
        {
            var loanCard = await _context.LoanCards.FindAsync(id);

            if (loanCard == null)
            {
                return NotFound("Could not find a loan card with that id");
            }
            return loanCard.ToLoanCardDTO();
        }

        
        // POST: api/LoanCard
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Add-Loan-Card")]
        public async Task<ActionResult<LoanCardDTO>> PostLoanCard(LoanCardCreateDTO loanCardCreateDTO)
        {
            var loanCard = loanCardCreateDTO.ToLoanCard(_context);
            _context.LoanCards.Add(loanCard);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLoanCard), new { id = loanCard.Id }, loanCard.ToLoanCardDTO());
        }

        private bool LoanCardExists(int id)
        {
            return _context.LoanCards.Any(e => e.Id == id);
        } 
    }
}
