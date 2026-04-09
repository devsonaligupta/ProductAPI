using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductAPI.Data;
using ProductAPI.DTOs;
using ProductAPI.Models;

namespace ProductAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(AuthRequestDto request, string role = "User");
        Task<AuthResponseDto?> LoginAsync(AuthRequestDto request);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        
        public async Task<AuthResponseDto?> RegisterAsync(AuthRequestDto request, string role = "User")
        {
           
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (existingUser != null)
                return null; 

           
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = hashedPassword,
                Role = role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

           
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(AuthRequestDto request)
        {
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
                return null; 

           
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isPasswordCorrect)
                return null; 

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = user.Role
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"]!;
            var jwtIssuer = _configuration["Jwt:Issuer"]!;
            var jwtAudience = _configuration["Jwt:Audience"]!;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

           
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role) 
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), 
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
