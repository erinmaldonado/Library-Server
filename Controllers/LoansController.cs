using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryModel;
using Server.DTOs;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly LibraryModelContext _context;

        public LoansController(LibraryModelContext context)
        {
            _context = context;
        }

        // GET: api/Loans — Administrator only
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAll()
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Select(l => new LoanResponse
                {
                    Id = l.Id,
                    BookId = l.BookId,
                    BookTitle = l.Book!.Title,
                    UserId = l.UserId,
                    UserEmail = l.User!.Email!,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    Status = l.Status.ToString()
                })
                .ToListAsync();

            return Ok(loans);
        }

        // GET: api/Loans/5 — Administrator only
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetById(int id)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.Id == id)
                .Select(l => new LoanResponse
                {
                    Id = l.Id,
                    BookId = l.BookId,
                    BookTitle = l.Book!.Title,
                    UserId = l.UserId,
                    UserEmail = l.User!.Email!,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    Status = l.Status.ToString()
                })
                .FirstOrDefaultAsync();

            if (loan == null) return NotFound();
            return Ok(loan);
        }

        // GET: api/Loans/user/{userId} — any logged-in user
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.UserId == userId)
                .Select(l => new LoanResponse
                {
                    Id = l.Id,
                    BookId = l.BookId,
                    BookTitle = l.Book!.Title,
                    UserId = l.UserId,
                    UserEmail = l.User!.Email!,
                    LoanDate = l.LoanDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    Status = l.Status.ToString()
                })
                .ToListAsync();

            return Ok(loans);
        }

        // POST: api/Loans — any logged-in user
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateLoanRequest request)
        {
            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null)
                return NotFound(new { message = "Book not found" });

            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var activeLoan = await _context.Loans
                .AnyAsync(l => l.BookId == request.BookId && l.Status == LoanStatus.Active);
            if (activeLoan)
                return BadRequest(new { message = "Book is already on loan" });

            var loan = new Loan
            {
                BookId = request.BookId,
                UserId = request.UserId,
                LoanDate = DateTime.UtcNow,
                DueDate = request.DueDate,
                Status = LoanStatus.Active
            };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, new { id = loan.Id });
        }

        // PUT: api/Loans/5/return — any logged-in user
        [HttpPut("{id}/return")]
        [Authorize]
        public async Task<IActionResult> Return(int id, ReturnLoanRequest request)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return NotFound();
            if (loan.Status == LoanStatus.Returned)
                return BadRequest(new { message = "Book already returned" });

            loan.ReturnDate = request.ReturnDate;
            loan.Status = LoanStatus.Returned;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Book returned successfully" });
        }

        // PUT: api/Loans/update-overdue — Administrator only
        [HttpPut("update-overdue")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateOverdue()
        {
            var overdueLoans = await _context.Loans
                .Where(l => l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow)
                .ToListAsync();

            foreach (var loan in overdueLoans)
                loan.Status = LoanStatus.Overdue;

            await _context.SaveChangesAsync();
            return Ok(new { message = $"{overdueLoans.Count} loans marked as overdue" });
        }

        // DELETE: api/Loans/5 — Administrator only
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return NotFound();

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}