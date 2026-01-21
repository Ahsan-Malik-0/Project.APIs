using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;

namespace Project.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresidentController(EventService _eventService, MemberService memberService) : ControllerBase
    {
        //Show all pending events
        [HttpGet("pendingEvents")]
        public async Task<IActionResult> GetPendingEvents()
        {
            var pendingEvents = await _eventService.GetPendingEvents();
            return Ok(pendingEvents);
        }

        //Add an event
        [HttpPost("addEvent")]
        public async Task<IActionResult> AddEvent([FromBody] AddEventByPresidentDto newEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _eventService.AddEventByPresident(newEvent);
            return Created();
        }

        //Update Existing 
        [HttpPut("updateEvent")]
        public async Task<IActionResult> UpdateEvent([FromBody] UpdateEventDto updateEventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _eventService.UpdateEventWithRequirements(updateEventDto);
            return Ok();
        }

        //Delete Event
        [HttpDelete("deleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            await _eventService.DeleteEventWithRequirements(id);
            return NoContent(); // 204
        }

        //Get all Events
        [HttpGet("history")]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventService.GetAllEvents();
            return Ok(events);
        }

        // View Profile
        [HttpGet("viewProfile/{id:guid}")]
        public async Task<IActionResult> GetProfile(Guid id)
        {
            var member = await memberService.ViewProfile(id);
            return Ok(member);
        }

        [HttpPut("updateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateMemberProfileDto updateMemberProfileDto)
        {
            if (!ModelState.IsValid) 
                return BadRequest("Fill the credentials");

            await memberService.EditProfile(updateMemberProfileDto);
            return Ok();
        }

    }
}
