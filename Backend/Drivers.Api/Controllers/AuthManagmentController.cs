using Drivers.Api.Configurations;
using Drivers.Api.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Drivers.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthManagmentController: ControllerBase
    {
        private readonly ILogger<AuthManagmentController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AuthManagmentController(ILogger<AuthManagmentController> logger, UserManager<IdentityUser> userManager,IOptionsMonitor<JwtConfig> jwtConfig)
        {
            _logger = logger;
            _userManager = userManager;
            _jwtConfig = jwtConfig.CurrentValue;
        }

        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto userRegistrationDto)
        {
            //Check if email exists
            var emailExists = await _userManager.FindByEmailAsync(userRegistrationDto.Email);
            if (!(emailExists is null)) { return BadRequest("Email Already Exists"); }

            var newUser = new IdentityUser()
            {
                Email = userRegistrationDto.Email,
                UserName = userRegistrationDto.Email,
            };
            
            var userIsCreated =  await _userManager.CreateAsync(newUser,userRegistrationDto.Password);
            if(userIsCreated.Succeeded)
            {
                var token = GenerateJwtToken(newUser);
                return Ok(new RegistrationRequestResponse()
                {
                    Result = true,
                    Token=token
                });
            }
            return BadRequest(userIsCreated.Errors.Select(x=>x.Description).ToList());
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto requestDto)
        {
            var existingUser =  await _userManager.FindByEmailAsync(requestDto.Email);
            if (existingUser is null)
            {
                return BadRequest("Invalid authentication");
            }
            var isPasswordValid = await _userManager.CheckPasswordAsync(existingUser,requestDto.Password);
            if (!isPasswordValid) { return BadRequest("Invalid authentication"); }
            var token = GenerateJwtToken(existingUser);
            return Ok(new LoginRequestResponse()
            {
                Result = true,
                Token = token
            });

        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
    }
}
