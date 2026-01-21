using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.IdentityModel.Tokens;
using Project.APIs.Database;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        public async Task<ActionResult<string>> GetMemberByPass(MemberLoginDto request)
        { 
            var token = await authService.LoginAsync(request);

            if (token == null)
            {
                return BadRequest("Invalid Usrename or password");
            }

            return Ok(token);
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

        [Authorize(Roles = "president")]
        [HttpGet("President-Only")]
        public IActionResult PresidentOnlyEndPoint()
        {
            return Ok("You are President");
        }
    }
}
