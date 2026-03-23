using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;

namespace Project.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChairPersonController(EventService _eventService, EventRequisitionService eventRequisitionService, MemberService memberService) : ControllerBase
    {
        // Handling Event Endpoints -------------------------------------------------------
        // Pending Events
        [HttpGet("pendingEvents")]
        public async Task<IActionResult> GetPendingEvents(Guid memberId)
        {
            var pendingEvents = await _eventService.GetPendingEvents(memberId);
            return Ok(pendingEvents);
        }

        //Update Existing 
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

        //Delete Event
        [HttpDelete("deleteRequestedEvent")]
        public async Task<IActionResult> DeleteEvent(Guid eventId)
        {
            await _eventService.DeleteEventWithRequirements(eventId);
            return NoContent(); // 204
        }

        // Reject Event
        [HttpPut("rejectEvent")]
        public async Task<IActionResult> RejectEvent([FromBody] UpdateEventStatusDto updateEventStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Fill the credentials");
            }

            await _eventService.UpdateEventStatus(updateEventStatus);
            return Ok();
        }

        // Temperory Delete Event
        [HttpPut("temperoryDeleteEvent")]
        public async Task<IActionResult> PutEventInWaititngState(Guid eventId)
        {

            UpdateEventStatusDto updateEventStatus = new UpdateEventStatusDto()
            {
                Id = eventId,
                Status = "waiting",
            };

            await _eventService.UpdateEventStatus(updateEventStatus);
            return Ok();
        }

        // Get specific event (use for edit event)
        [HttpGet("getEventById")]
        public async Task<IActionResult> GetEventById(Guid eventId)
        {
            if (eventId == Guid.Empty)
                return BadRequest();

            var @event = await _eventService.GetEventById(eventId);

            return Ok(@event);
        }




        // Handling Requisition Endpoints -----------------------------------------------------
        //Pending Requisitions
        [HttpGet("getPendingRequisitions")]
        public async Task<IActionResult> PendingRequisitions(Guid memberId)
        {
            var pendingRequisitions = await eventRequisitionService.GetPendingEventRequisitions(memberId);
            return Ok(pendingRequisitions);
        }

        //Requisition Detail
        [HttpGet("getEventRequisitionDetail")]
        public async Task<IActionResult> GetEventRequisitionDetails(Guid requisitionId)
        {
            var requisitionsDetails= await eventRequisitionService.GetEventRequisitionDetails(requisitionId);
            return Ok(requisitionsDetails);
        }

        //Create Requisition
        [HttpPost("createEventRequisition")]
        public async Task<IActionResult> CreateEventRequisition([FromBody] CreateEventRequisitionDto requisitionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await eventRequisitionService.CreateRequisition(requisitionDto);
            return Ok();
        }

        //Get Chairperson details for requisition form
        [HttpGet("detailsForRequisition")]
        public async Task<IActionResult> GetDetailsForRequisitionForm(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var chairpersonDetail = await memberService.GetChairpersonDetailsForRequisitionForm(id);
            return Ok(chairpersonDetail);
        }


        //Get event Requirements

        //View Rquisitions History

        //View Profile
        [HttpGet("viewProfile")]
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

        
    }
}