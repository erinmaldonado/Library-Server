using Microsoft.AspNetCore.Identity;

namespace LibraryModel
{
    public class LibraryModelUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
