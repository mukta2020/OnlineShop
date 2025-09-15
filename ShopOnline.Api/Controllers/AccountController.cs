using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Extensions;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Api.Services.Contracts;
using ShopOnline.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopOnline.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenService _refreshTokenService; // Inject your refresh token service here

        public AccountController(UserManager<User> userManager, IConfiguration configuration, IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            var user = await _userManager.FindByNameAsync(userForAuthentication.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
            {
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication" });
            }

            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims(user);
            var accessToken = GenerateAccessToken(signingCredentials, claims);
            var refreshToken = GenerateRefreshToken();

            // Associate the refresh token with the user
            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponseDto
            {
                IsAuthSuccessful = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequest)
        {
            var user = await _userManager.FindByNameAsync(refreshTokenRequest.Email);

            if (user == null || user.RefreshToken != refreshTokenRequest.RefreshToken)
            {
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid refresh token" });
            }

            // Check if the refresh token is still valid
            if (!_refreshTokenService.IsRefreshTokenValid(refreshTokenRequest.RefreshToken))
            {
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Refresh token expired" });
            }

            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims(user);
            var accessToken = GenerateAccessToken(signingCredentials, claims);

            return Ok(new AuthResponseDto
            {
                IsAuthSuccessful = true,
                AccessToken = accessToken
            });
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                UserName = registrationDto.Email,
                Email = registrationDto.Email,
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName
            };

            var result = await _userManager.CreateAsync(user, registrationDto.Password);

            if (result.Succeeded)
            {
                // You can optionally generate a confirmation email here and send it to the user
                // or automatically confirm the user if your application doesn't require email confirmation.
                return Ok(new { Message = "User registration successful" });
            }
            else
            {
                return BadRequest(new { Errors = result.Errors });
            }
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);
            var secretKey = new SymmetricSecurityKey(key);
            return new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        }

        private List<Claim> GetClaims(IdentityUser user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            // Add more claims as needed
        };

            return claims;
        }

        private string GenerateAccessToken(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:AccessExpirationMinutes"])),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private string GenerateRefreshToken()
        {
            // Generate a unique refresh token (you can use a GUID or any other method)
            return Guid.NewGuid().ToString();
        }

    }
}
