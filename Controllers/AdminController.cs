using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using LibraryModel;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<LibraryModelUser> _userManager;
        private readonly JwtHandler _jwtHandler;

        public AdminController(UserManager<LibraryModelUser> userManager, JwtHandler jwtHandler)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(request.Email) || 
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "Email and password are required" });
                }

                var user = new LibraryModelUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FullName = request.FullName,
                    EmailConfirmed = true
                };

                // Create user in DB (hashes password automatically)
                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    // Add to RegisteredUser role by default
                    await _userManager.AddToRoleAsync(user, "RegisteredUser");

                    return Ok(new { 
                        success = true,
                        message = "Registration successful" 
                    });
                }

                // If it fails (e.g. password too weak, email taken), return errors
                return BadRequest(new { 
                    success = false,
                    errors = result.Errors.Select(e => e.Description).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false,
                    message = "Registration failed",
                    error = ex.Message 
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(loginRequest.Username) || 
                    string.IsNullOrWhiteSpace(loginRequest.Password))
                {
                    return BadRequest(new LoginResponse
                    {
                        Success = false,
                        Message = "Username and password are required",
                        Token = ""
                    });
                }

                // Find user
                var user = await _userManager.FindByNameAsync(loginRequest.Username);
                if (user == null)
                {
                    // Also try to find by email
                    user = await _userManager.FindByEmailAsync(loginRequest.Username);
                }

                if (user == null)
                {
                    return Unauthorized(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password",
                        Token = ""
                    });
                }

                // Check password
                bool loginStatus = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
                if (!loginStatus)
                {
                    return Unauthorized(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password",
                        Token = ""
                    });
                }

                // Generate JWT Token
                JwtSecurityToken jwtToken = await _jwtHandler.GenerateTokenAsync(user);
                string stringToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = stringToken
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new LoginResponse
                {
                    Success = false,
                    Message = "Login failed",
                    Token = ""
                });
            }
        }
    }
}