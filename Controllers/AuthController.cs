using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;

namespace Project.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, MemberService memberService) : ControllerBase
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

            var token = await authService.LoginAsync(request.Username, request.HashPassword);
            return Ok(token);
        }

        //View Profile
        [HttpGet("viewProfile/{memberId:guid}")]
        public async Task<IActionResult> GetProfile(Guid memberId)
        {
            var member = await memberService.ViewProfile(memberId);
            return Ok(member);
        }

        //Edit Profile
        [HttpPut("updateProfile/{memberId:guid}")]
        public async Task<IActionResult> UpdateProfile(Guid memberId, [FromBody] UpdateMemberProfileDto updateMemberProfileDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Fill the credentials");

            await memberService.EditProfile(memberId, updateMemberProfileDto);
            return Ok();
        }

        // View Member Profile
        [HttpGet("viewMemberProfile/{upperLevelId:guid}")]
        public async Task<IActionResult> GetPresidentProfile(Guid upperLevelId)
        {
            var lowerMember = await memberService.GetMemberProfile(upperLevelId);
            return Ok(lowerMember);
        }

        // Edit Member Profile
        [HttpPut("updateMemberProfile/{lowerLevelId:guid}")]
        public async Task<IActionResult> UpdatePresidentProfile(Guid lowerLevelId, EditMemberProfileDto editMemberProfile)
        {
            await memberService.UpdateMemberProfile(lowerLevelId, editMemberProfile);
            return Ok();
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
