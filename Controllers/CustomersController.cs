using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
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
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet("View-All-Customers")]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetCustomers()
        {
            return await _context.Customers
                .Include(x => x.Ratings)
                .Select(x => x.ToCustomerDTO()).AsNoTracking().ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("Search-Customer-By-Id")]
        public async Task<ActionResult<CustomerDTO>> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(x => x.Ratings)
                .Where(x => x.Id.Equals(id))
                .FirstOrDefaultAsync();

            if (customer == null)
            {
                return NotFound("Could not find a customer with that id");
            }

            return customer.ToCustomerDTO();
        } 

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPut("Edit-Customer-By-Id")]
        public async Task<IActionResult> PutCustomer(int customerId, string firstName, string lastName)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                return NotFound("Could not find a customer with that id");
            }

            customer.FirstName = firstName;
            customer.LastName = lastName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customerId))
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

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Add-Customer")]
        public async Task<ActionResult<CustomerDTO>> PostCustomer(CustomerCreateDTO customerCreateDTO)
        {
            var customer = customerCreateDTO.ToCustomer(_context);

            int newNumber = _context.LoanCards.Count() == 0 ? newNumber = 1000 : newNumber = _context.LoanCards.Max(x => x.LoanCardNumber) + 1;

            LoanCard loanCard = new LoanCard
            {
                Customer = customer,
                LoanCardNumber = newNumber
            };

            customer.LoanCard = loanCard;
            _context.Customers.Add(customer);
            _context.LoanCards.Add(loanCard);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer.ToCustomerDTO());
        }

        // DELETE: api/Customers/5
        [HttpDelete("Delete-Customer-By-Id")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound("Could not find a customer with that id to delete");
            }

            var loanCard = await _context.LoanCards.FirstOrDefaultAsync(x => x.Id.Equals(customer.LoanCardId));

            if (loanCard != null)
            {
                _context.LoanCards.Remove(loanCard);
            }
            
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok("Customer deleted successfully");
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
