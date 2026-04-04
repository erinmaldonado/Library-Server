using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryModel
{
    public class LibraryModelContext : IdentityDbContext<LibraryModelUser>
    {
        public LibraryModelContext(DbContextOptions<LibraryModelContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Loan> Loans { get; set; }
    }
}
