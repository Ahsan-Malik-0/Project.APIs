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
        [HttpGet("pendingEvents/{memberId:guid}")]
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
        [HttpDelete("deleteRequestedEvent/{eventId:guid}")]
        public async Task<IActionResult> DeleteEvent(Guid eventId)
        {
            await _eventService.DeleteEventWithRequirements(eventId);
            return NoContent(); // 204
        }

        // Reject Event
        [HttpPut("rejectEvent/{eventId:guid}")]
        public async Task<IActionResult> RejectEvent(Guid eventId, [FromBody] UpdateEventStatusDto updateEventStatus)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Fill the credentials");
            }

            await _eventService.UpdateEventStatus(eventId, updateEventStatus);
            return Ok();
        }

        // Put event in waiting state
        [HttpPut("temperoryRemoveEvent/{eventId:guid}")]
        public async Task<IActionResult> PutEventInWaititngState(Guid eventId)
        {
            UpdateEventStatusDto updateEventStatus = new UpdateEventStatusDto()
            {
                Status = "waiting",
            };

            await _eventService.UpdateEventStatus(eventId, updateEventStatus);
            return Ok();
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


        // Handling Requisition Endpoints -----------------------------------------------------
        //Pending Requisitions
        [HttpGet("getPendingRequisitions/{memberId:guid}")]
        public async Task<IActionResult> PendingRequisitions(Guid memberId)
        {
            var pendingRequisitions = await eventRequisitionService.GetPendingEventRequisitions(memberId);
            return Ok(pendingRequisitions);
        }

        //Requisition Detail
        [HttpGet("getEventRequisitionDetail/{requisitionId:guid}")]
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
        [HttpGet("detailsForRequisition/{memberId:guid}")]
        public async Task<IActionResult> GetDetailsForRequisitionForm(Guid memberId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var chairpersonDetail = await memberService.GetChairpersonDetailsForRequisitionForm(memberId);
            return Ok(chairpersonDetail);
        }


        //Get event Requirements

        //View Rquisitions History

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

        
    }
}