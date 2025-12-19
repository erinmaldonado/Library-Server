using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryModel;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly LibraryModelContext _context;

        public ImportController(LibraryModelContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("books-csv")]
        public async Task<IActionResult> ImportBooksFromCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var authorsAdded = 0;
            var booksAdded = 0;
            var authorCache = new Dictionary<string, Author>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null
            }))
            {
                await csv.ReadAsync();
                csv.ReadHeader();

                while (await csv.ReadAsync())
                {
                    try
                    {
                        var title = csv.GetField<string>("Title")?.Trim();
                        var authorsField = csv.GetField<string>("Authors")?.Trim();
                        var description = csv.GetField<string>("Description")?.Trim();
                        var category = csv.GetField<string>("Category")?.Trim();
                        var publisher = csv.GetField<string>("Publisher")?.Trim();
                        var priceStr = csv.GetField<string>("Price Starting With ($)")?.Trim();
                        var monthStr = csv.GetField<string>("Publish Date (Month)")?.Trim();
                        var yearStr = csv.GetField<string>("Publish Date (Year)")?.Trim();

                        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(authorsField))
                            continue;

                        // Parse author name (remove "By " prefix if present)
                        var authorName = authorsField.Replace("By ", "").Trim();
                        // Take first author if multiple (split by " and " or ",")
                        if (authorName.Contains(" and "))
                            authorName = authorName.Split(" and ")[0].Trim();
                        if (authorName.Contains(","))
                            authorName = authorName.Split(',')[0].Trim();

                        if (string.IsNullOrEmpty(authorName))
                            continue;

                        // Get or create author
                        if (!authorCache.ContainsKey(authorName))
                        {
                            var existingAuthor = _context.Authors.FirstOrDefault(a => a.Name == authorName);
                            if (existingAuthor != null)
                            {
                                authorCache[authorName] = existingAuthor;
                            }
                            else
                            {
                                var newAuthor = new Author { Name = authorName };
                                _context.Authors.Add(newAuthor);
                                await _context.SaveChangesAsync();
                                authorCache[authorName] = newAuthor;
                                authorsAdded++;
                            }
                        }

                        var author = authorCache[authorName];

                        // Parse price
                        decimal? price = null;
                        if (!string.IsNullOrEmpty(priceStr) && decimal.TryParse(priceStr, out var parsedPrice))
                            price = parsedPrice;

                        // Parse year
                        int? year = null;
                        if (!string.IsNullOrEmpty(yearStr) && int.TryParse(yearStr, out var parsedYear))
                            year = parsedYear;

                        // Create book
                        var book = new Book
                        {
                            Title = title,
                            Description = string.IsNullOrEmpty(description) ? null : description,
                            Category = string.IsNullOrEmpty(category) ? null : category,
                            Publisher = string.IsNullOrEmpty(publisher) ? null : publisher,
                            Price = price,
                            PublishMonth = string.IsNullOrEmpty(monthStr) ? null : monthStr,
                            PublishYear = year,
                            AuthorId = author.Id
                        };

                        _context.Books.Add(book);
                        booksAdded++;
                    }
                    catch (Exception ex)
                    {
                        // Skip rows with errors
                        continue;
                    }
                }

                await _context.SaveChangesAsync();
            }

            return Ok(new { authorsAdded, booksAdded });
        }
    }
}