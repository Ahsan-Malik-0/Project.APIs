using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;
using static Project.APIs.Model.DTOs.AdministrationDto;

namespace Project.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        //Registration
        [HttpPost("Register")]
        public async Task<ActionResult<Member>> AddMember([FromBody] MemberDto memberDto)
        {
            var member = await authService.RegisterAsync(memberDto);

            if (member == null) {
                return BadRequest("User Already Exist");
            }

            return Ok(member);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> GetMemberByPass(LoginDto request)
        {

            var token1 = await authService.LoginAsync(request.Username, request.HashPassword);

            if (token1 == null)
            {
                var token2 = await authService.LoginAdminAsync(request.Username, request.HashPassword);
                if (token2 == null)
                {
                    return BadRequest("Invalid Username or password");
                }
                return Ok(token2);
            }

            return Ok(token1);
        }

        [HttpPost("AdminRegister")]
        public async Task<ActionResult<Administration>> AddAdmin([FromBody] CreateAdministrationDto administrationDto)
        {
            var admin = await authService.RegisterAdminAsync(administrationDto);

            if (admin == null)
            {
                return BadRequest("User Already Exist");
            }

            return Ok(admin);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("You are authonticated");
        }

        [Authorize(Roles = "admin")]
        [HttpGet("Admin-Only")] 
        public IActionResult AdminOnlyEndPoint()
        {
            return Ok("You are admin");
        }

        [Authorize(Roles = "CP")]
        [HttpGet("CP-Only")]
        public IActionResult PresidentOnlyEndPoint()
        {
            return Ok("You are CP");
        }
    }
}
