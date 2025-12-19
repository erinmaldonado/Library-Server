using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryModel;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly LibraryModelContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<LibraryModelUser> _userManager;
        private readonly IConfiguration _configuration;

        public SeedController(
            LibraryModelContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<LibraryModelUser> userManager,
            IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        // GET: api/Seed
        [HttpGet]
        public async Task<ActionResult> CreateDefaultUsers()
        {
            // Setup the default role names
            string role_RegisteredUser = "RegisteredUser";
            string role_Administrator = "Administrator";

            // Create the default roles (if they don't exist yet)
            if (await _roleManager.FindByNameAsync(role_RegisteredUser) == null)
                await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));

            if (await _roleManager.FindByNameAsync(role_Administrator) == null)
                await _roleManager.CreateAsync(new IdentityRole(role_Administrator));

            // Create a list to track the newly added users
            var addedUserList = new List<LibraryModelUser>();

            // Check if the admin user already exists
            var email_Admin = "admin@email.com";
            if (await _userManager.FindByNameAsync(email_Admin) == null)
            {
                // Create a new admin LibraryModelUser account
                var user_Admin = new LibraryModelUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_Admin,
                    Email = email_Admin,
                    FullName = "Administrator"
                };

                // Get password from configuration
                var adminPassword = _configuration["DefaultPasswords:Administrator"] ?? "Admin@123";

                // Insert the admin user into the DB
                var result = await _userManager.CreateAsync(user_Admin, adminPassword);

                if (result.Succeeded)
                {
                    // Assign the "RegisteredUser" and "Administrator" roles
                    await _userManager.AddToRoleAsync(user_Admin, role_RegisteredUser);
                    await _userManager.AddToRoleAsync(user_Admin, role_Administrator);

                    // Confirm the e-mail and remove lockout
                    user_Admin.EmailConfirmed = true;
                    user_Admin.LockoutEnabled = false;

                    // Add the admin user to the added users list
                    addedUserList.Add(user_Admin);
                }
            }

            // Check if the standard user already exists
            var email_User = "user@email.com";
            if (await _userManager.FindByNameAsync(email_User) == null)
            {
                // Create a new standard LibraryModelUser account
                var user_User = new LibraryModelUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_User,
                    Email = email_User,
                    FullName = "Regular User"
                };

                // Get password from configuration
                var userPassword = _configuration["DefaultPasswords:RegisteredUser"] ?? "User@123";

                // Insert the standard user into the DB
                var result = await _userManager.CreateAsync(user_User, userPassword);

                if (result.Succeeded)
                {
                    // Assign the "RegisteredUser" role
                    await _userManager.AddToRoleAsync(user_User, role_RegisteredUser);

                    // Confirm the e-mail and remove lockout
                    user_User.EmailConfirmed = true;
                    user_User.LockoutEnabled = false;

                    // Add the standard user to the added users list
                    addedUserList.Add(user_User);
                }
            }

            // If we added at least one user, persist the changes into the DB
            if (addedUserList.Count > 0)
                await _context.SaveChangesAsync();

            return Ok(new
            {
                Count = addedUserList.Count,
                Users = addedUserList.Select(u => new { u.Email, u.UserName, u.FullName })
            });
        }
    }
}