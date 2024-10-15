using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly BookstoreDbContext _context;

        public AuthController(IConfiguration config, BookstoreDbContext context)
        {
            _config = config;
            _context = context;
        }

        // User login
        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User login)
        {
            var user = await AuthenticateUserAsync(login); 

            if (user != null)
            {
                var token = GenerateJWT(user);
                return Ok(new { token });
            }

            return Unauthorized("Invalid credentials");
        }

        // Authenticating the user against the database
        private async Task<User> AuthenticateUserAsync(User login)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == login.Username);

            if (user != null && VerifyPassword(login.Password, user.Password))
            {
                return user; 
            }
            return null; 
        }

        // method for verify password 
        private bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            var hashedInput = HashPassword(inputPassword); 
            return hashedInput == storedHashedPassword; 
        }

        // Generation of JWT Token
        private string GenerateJWT(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "User"), 
                new Claim(JwtRegisteredClaimNames.Sub, user.Password),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60), 
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token); 
        }

        // New user registration
        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                return BadRequest("Username is already taken.");
            }

            user.Password = HashPassword(user.Password);

            // Saving the user
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }

        // method for hash passwords
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
