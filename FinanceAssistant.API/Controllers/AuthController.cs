using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FinanceAssistant.API.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using FinanceAssistant.API.Data;

namespace FinanceAssistant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly FinanceContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            FinanceContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Username kontrolü
            if (await _userManager.FindByNameAsync(model.Username) != null)
            {
                return BadRequest("Username is already taken.");
            }

            // Email kontrolü
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return BadRequest("Email address is already registered.");
            }

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmed = true // Email verification disabled for now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                
                // Create user settings
                var userSettings = new UserSettings
                {
                    UserId = user.Id,
                    IsTwoFactorEnabled = false,
                    CreatedAt = DateTime.UtcNow
                };
                
                _context.UserSettings.Add(userSettings);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User created successfully." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                var token = GenerateJwtToken(user);
                return Ok(new { 
                    token = token,
                    user = new {
                        id = user.Id,
                        username = user.UserName,
                        email = user.Email,
                        firstName = user.FirstName,
                        lastName = user.LastName
                    }
                });
            }

            if (result.RequiresTwoFactor)
            {
                return Ok(new { requiresTwoFactor = true });
            }

            return BadRequest("Invalid login attempt.");
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? "your-secret-key"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JWT:ExpireDays"] ?? "7"));

            var token = new JwtSecurityToken(
                _configuration["JWT:ValidIssuer"],
                _configuration["JWT:ValidAudience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class RegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
} 