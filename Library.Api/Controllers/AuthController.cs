using AutoMapper;
using Library.Api.CustomActionFilters;
using Library.Api.Models.Dto;
using Library.Api.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;
        private readonly IMapper mapper;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository, IMapper mapper)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var ideintityUser = new IdentityUser 
            { 
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };
            var identityResult = await userManager.CreateAsync(ideintityUser, registerRequestDto.Password);
            if (identityResult.Succeeded)
            {
                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any()) 
                {
                   identityResult = await userManager.AddToRolesAsync(ideintityUser, registerRequestDto.Roles);
                    if (identityResult.Succeeded)
                    {
                        return Ok("User is registerd!!!");
                    }
                }
            }
            return BadRequest(identityResult.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByIdAsync(loginRequestDto.Username);
            if (user != null)
            {
                var signInResult = await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (signInResult)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        var jwtToken = tokenRepository.createJWTToken(user, roles.ToList());
                        var respose = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };
                        return Ok(respose);
                    }
                }
            }
            return BadRequest("Invalid login attempt");

        }
    }
}
