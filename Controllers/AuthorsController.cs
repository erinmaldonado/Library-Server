using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LibraryModel;
using Server.DTOs;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly LibraryModelContext _context;

        public AuthorsController(LibraryModelContext context)
        {
            _context = context;
        }

        // GET: api/Authors
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
        {
            var authors = await _context.Authors
                .Select(a => new AuthorDto
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .ToListAsync();

            return Ok(authors);
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
        {
            var author = await _context.Authors
                .Where(a => a.Id == id)
                .Select(a => new AuthorDto
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .FirstOrDefaultAsync();

            if (author == null)
            {
                return NotFound(new { message = "Author not found" });
            }

            return author;
        }

        // GET: api/Authors/5/books
        [HttpGet("{id}/books")]
        [Authorize]
        public async Task<ActionResult<AuthorWithBooksDto>> GetAuthorWithBooks(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .Where(a => a.Id == id)
                .Select(a => new AuthorWithBooksDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Books = a.Books.Select(b => new BookDto
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Description = b.Description,
                        Category = b.Category,
                        Publisher = b.Publisher,
                        Price = b.Price,
                        PublishMonth = b.PublishMonth,
                        PublishYear = b.PublishYear,
                        AuthorId = b.AuthorId,
                        AuthorName = a.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (author == null)
            {
                return NotFound(new { message = "Author not found" });
            }

            return author;
        }

        // POST: api/Authors
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AuthorDto>> PostAuthor(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAuthor", new { id = author.Id }, new AuthorDto
            {
                Id = author.Id,
                Name = author.Name
            });
        }

        // PUT: api/Authors/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutAuthor(int id, Author author)
        {
            if (id != author.Id)
            {
                return BadRequest(new { message = "Author ID mismatch" });
            }

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
                {
                    return NotFound(new { message = "Author not found" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound(new { message = "Author not found" });
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}