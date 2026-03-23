using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        // Handling Event Endpoints --------------------------------------------------------
        //Show all pending events
        [HttpGet("pendingEvents/{memberId:guid}")]
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

        //Update Existing event
        [HttpPut("updateEvent/{eventId:guid}")]
        public async Task<IActionResult> UpdateEvent(Guid eventId, [FromBody] UpdateEventDto updateEventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _eventService.UpdateEventWithRequirements(eventId, updateEventDto);
            return Ok();
        }

        //Delete pending event
        [HttpDelete("deletePendingEvent/{eventId:guid}")]
        public async Task<IActionResult> DeleteEvent(Guid eventId)
        {
            await _eventService.DeleteEventWithRequirements(eventId);
            return NoContent(); // 204
        }

        //Get all accepted Events for history
        [HttpGet("history/{memberId:guid}")]
        public async Task<IActionResult> GetEventsHistory(Guid memberId)
        {
            var events = await _eventService.GetEventsHistory(memberId);
            return Ok(events);
        }

        // Get specific event (use for edit event)
        [HttpGet("getEventById/{eventId:guid}")]
        public async Task<IActionResult> GetEventById(Guid eventId)
        {
            if (eventId == Guid.Empty)
                return BadRequest();

            var @event = await _eventService.GetEventById(eventId);

            return Ok(@event);
        }


        // Handling Profile Endpoints ----------------------------------------------------------
        // View Profile
        [HttpGet("viewProfile/{memberId:guid}")]
        public async Task<IActionResult> GetProfile(Guid memberId)
        {
            var member = await memberService.ViewProfile(memberId);
            return Ok(member);
        }

        [HttpPut("updateProfile/{memberId:guid}")]
        public async Task<IActionResult> UpdateProfile(Guid memberId, [FromBody] UpdateMemberProfileDto updateMemberProfileDto)
        {
            if (!ModelState.IsValid) 
                return BadRequest("Fill the credentials");

            await memberService.EditProfile(memberId, updateMemberProfileDto);
            return Ok();
        }


        // Handling Society Endpoints ---------------------------------------------------------------
        // Get society id using member's id
        [HttpGet("getSocietyId/{memberId:guid}")]
        public async Task<IActionResult> GetSocietyId(Guid memberId)
        {
            if (memberId == Guid.Empty)
                return BadRequest();

            var societyId = await memberService.GetSocietyIdAsync(memberId);

            return Ok(societyId);
        }
    }
}
