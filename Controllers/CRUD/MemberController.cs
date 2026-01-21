using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Controllers.CRUD
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController(DB _dB) : ControllerBase
    {
        //private DB _dB;
        //private IPasswordHasher<Member> _passwordHasher;

        //public MemberController(
        //DB dB,
        //IPasswordHasher<Member> passwordHasher)
        //{
        //    _dB = dB;
        //    _passwordHasher = passwordHasher;
        //}

        [HttpGet]
        public async Task<IActionResult> GetMembers()
        {
            var members = await _dB.Members
                .Include(m => m.Society)
                .ToListAsync();
            return Ok(members);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetMemberById(Guid id)
        {
            var member = await _dB.Members
                .Include(m => m.Society)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null)
                return NotFound();

            return Ok(new
            {
                member.Id,
                member.Name,
                member.Username,
                member.Role,
                member.Picture,
                member.SocietyId,
                SocietyName = member.Society?.Name
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateMember([FromBody] MemberDto memberDto, Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { Error = "Id must be a non-empty GUID." });
            }

            if (memberDto is null)
            {
                return BadRequest(new { Error = "Request body is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var member = await _dB.Members.FindAsync(id);
            if (member is null)
            {
                return NotFound();
            }

            member.Name = memberDto.Name;
            member.Username = memberDto.Username;
            member.HashPassword = memberDto.HashPassword;
            member.Role = memberDto.Role;
            member.Picture = memberDto.Picture;

            _dB.Members.Update(member);
            await _dB.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteMember([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { Error = "Id must be a non-empty GUID" });
            }

            var member = await _dB.Members.FindAsync(id);
            if (member is null)
            {
                return NotFound();
            }

            _dB.Members.Remove(member);
            await _dB.SaveChangesAsync();
            return NoContent();
        }

        //[HttpGet("GetEvent")]
        //public IActionResult GetEvent()
        //{
        //    var result = 
        //}
    }
}
