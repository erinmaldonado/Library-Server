using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using LibraryModel;
using Server.DTOs;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryModelContext _context;

        public BooksController(LibraryModelContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            var skip = (page - 1) * pageSize;

            var books = await _context.Books
                .Include(b => b.Author)
                .OrderBy(b => b.Title)
                .Skip(skip)
                .Take(pageSize)
                .Select(b => new BookDto
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
                    AuthorName = b.Author != null ? b.Author.Name : null
                })
                .ToListAsync();

            return Ok(books);
        }

        // GET: api/Books/count
        [HttpGet("count")]
        [Authorize]
        public async Task<ActionResult<int>> GetBooksCount()
        {
            var count = await _context.Books.CountAsync();
            return Ok(new { count });
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Where(b => b.Id == id)
                .Select(b => new BookDto
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
                    AuthorName = b.Author != null ? b.Author.Name : null
                })
                .FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound(new { message = "Book not found" });
            }

            return book;
        }

        // GET: api/Books/ByAuthor/5
        [HttpGet("ByAuthor/{authorId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByAuthor(int authorId)
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Where(b => b.AuthorId == authorId)
                .Select(b => new BookDto
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
                    AuthorName = b.Author != null ? b.Author.Name : null
                })
                .ToListAsync();

            return Ok(books);
        }

        // GET: api/Books/ByCategory?category=name
        [HttpGet("ByCategory")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksByCategory([FromQuery] string category)
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Where(b => b.Category != null && b.Category.Contains(category))
                .Select(b => new BookDto
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
                    AuthorName = b.Author != null ? b.Author.Name : null
                })
                .ToListAsync();

            return Ok(books);
        }

        // PUT: api/Books/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest(new { message = "Book ID mismatch" });
            }

            var authorExists = await _context.Authors.AnyAsync(a => a.Id == book.AuthorId);
            if (!authorExists)
            {
                return BadRequest(new { message = "Author not found" });
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound(new { message = "Book not found" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Books
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            var authorExists = await _context.Authors.AnyAsync(a => a.Id == book.AuthorId);
            if (!authorExists)
            {
                return BadRequest(new { message = "Author not found" });
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBook", new { id = book.Id }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound(new { message = "Book not found" });
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}