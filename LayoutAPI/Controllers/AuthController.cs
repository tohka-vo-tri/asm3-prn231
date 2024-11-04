using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repository;
using DataAccess.Models;
using LayoutAPI.DTO.Request;

namespace LayoutAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ViroCureUserRepository _viroCureUserRepository;
        private IConfiguration _configuration;

        public AuthController(ViroCureUserRepository viroCureUserRepository,IConfiguration configuration)
        {
            _configuration = configuration;
            _viroCureUserRepository = viroCureUserRepository;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest loginRequest)
        {
            var checkLogin = _viroCureUserRepository.CheckLogin(loginRequest.Email, loginRequest.Password);
            if (checkLogin != null)
            {
                var token = GenerateJSONWebToken(checkLogin);

                var response = new
                {
                    message = "Login successful",
                    token = token,
                    user = new
                    {
                        id = checkLogin.UserId,
                        email = checkLogin.Email,
                        role = checkLogin.Role
                    }
                };

                return Ok(response);
            }

            return Unauthorized(new { error = "Invalid email or password" });
        }

        private string GenerateJSONWebToken(ViroCureUser login)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, login.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        // Add role claim based on username
        new Claim(ClaimTypes.Role, login.Role.ToString())
    };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
