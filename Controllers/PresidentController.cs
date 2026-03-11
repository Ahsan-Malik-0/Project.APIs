using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;
using System.ComponentModel;

namespace Project.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresidentController(EventService _eventService, MemberService memberService) : ControllerBase
    {
        //Show all pending events
        [HttpGet("pendingEvents")]
        public async Task<IActionResult> GetPendingAndRejectedEvents(Guid memberId)
        {
            var pendingEvents = await _eventService.GetPendingAndRejectedEvents(memberId);
            return Ok(pendingEvents);
        }

        //Add an event
        [HttpPost("addEvent")]
        public async Task<IActionResult> AddEvent([FromBody] AddEventDto newEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _eventService.AddEvent(newEvent, "pending");
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
        [HttpPut("softDeleteEvent")]
        public async Task<IActionResult> SoftDeleteEvent([FromBody] Event @event)
        {
            await _eventService.SoftDeleteEventWithRequirements(@event.Id);
            return Ok(); // 204
        }

        [HttpDelete("permanetDeleteEvent/{id}")]
        public async Task<IActionResult> PermanentDeleteEvent(Guid eventId)
        {
            await _eventService.PermanentDeleteEventWithRequirements(eventId);
            return NoContent(); // 204
        }

        //Get all accepted Events for history
        [HttpGet("history")]
        public async Task<IActionResult> GetEventsHistory(Guid memberId)
        {
            var events = await _eventService.GetEventsHistory(memberId);
            return Ok(events);
        }

        // View Profile
        [HttpGet("viewProfile")]
        public async Task<IActionResult> GetProfile(Guid memberId)
        {
            var member = await memberService.ViewProfile(memberId);
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

        // Get society id using member's id
        [HttpGet("getSocietyId")]
        public async Task<IActionResult> GetSocietyId(Guid memberId)
        {
            if (memberId == Guid.Empty)
                return BadRequest();

            var societyId = await memberService.GetSocietyIdAsync(memberId);

            return Ok(societyId);
        }

        // Get specific event (use for edit event)
        [HttpGet("getEventById")]
        public async Task<IActionResult> GetEventById(Guid eventId)
        {
            if(eventId == Guid.Empty)
                return BadRequest();

            var @event = await _eventService.GetEventById(eventId);

            return Ok(@event);
        }

    }
}
