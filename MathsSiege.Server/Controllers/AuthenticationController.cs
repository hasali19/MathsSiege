using MathsSiege.Models;
using MathsSiege.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using BCryptHasher = BCrypt.Net.BCrypt;

namespace MathsSiege.Server.Controllers
{
    [Route("Api/Auth")]
    public class AuthenticationController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IConfiguration configuration;

        public AuthenticationController(IUserRepository userRepository, IConfiguration configuration)
        {
            this.userRepository = userRepository;
            this.configuration = configuration;
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User credentials)
        {
            if (string.IsNullOrEmpty(credentials.Username)
                || string.IsNullOrEmpty(credentials.Password))
            {
                return BadRequest();
            }

            var user = await userRepository.GetUserAsync(credentials.Username);

            if (user == null)
            {
                return NotFound();
            }

            var match = BCryptHasher.Verify(credentials.Password, user.Password);

            if (!match)
            {
                return Unauthorized();
            }

            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            
            ClaimsIdentity identity = new ClaimsIdentity(claims, "Password");
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecretKey"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: configuration["JwtOptions:Issuer"],
                audience: configuration["JwtOptions:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(4),
                signingCredentials: signingCredentials);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
