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
    public class ChairPersonController(EventService _eventService, EventRequisitionService eventRequisitionService, MemberService memberService, YearlyBudgetService yearlyBudgetService) : ControllerBase
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
        [HttpPut("postponeEvent/{eventId:guid}")]
        public async Task<IActionResult> PostponeEvent(Guid eventId)
        {
            UpdateEventStatusDto updateEventStatus = new UpdateEventStatusDto()
            {
                Status = "postponed",
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


        // Handling Events Requisition Endpoints -----------------------------------------------------
        // Pending Envents Requisitions
        [HttpGet("getPendingEventRequisitions/{memberId:guid}")]
        public async Task<IActionResult> PendingRequisitions(Guid memberId)
        {
            var pendingRequisitions = await eventRequisitionService.GetPendingEventRequisitions(memberId);
            return Ok(pendingRequisitions);
        }

        // Events Requisitions Detail
        [HttpGet("getEventRequisitionDetails/{requisitionId:guid}")]
        public async Task<IActionResult> GetEventRequisitionDetails(Guid requisitionId)
        {
            var requisitionsDetails = await eventRequisitionService.GetEventRequisitionDetails(requisitionId);
            return Ok(requisitionsDetails);
        }

        [HttpGet("getChairpersonDetailsForRequisition/{chairpersonId:guid}")]
        public async Task<IActionResult> GetChaipersonDetailsForRequisition(Guid chairpersonId)
        {
            var chairpersonDetails = await memberService.GetChairpersonDetailsForRequisition(chairpersonId);
            return Ok(chairpersonDetails);
        }

        // Create Events Requisitions
        [HttpPost("createEventRequisition")]
        public async Task<IActionResult> CreateEventRequisition([FromBody] CreateEventRequisitionDto requisitionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await eventRequisitionService.CreateEventRequisition(requisitionDto);
            return Ok();
        }

        // Delete Events Requisitions
        [HttpDelete("deleteEventRequisition/{requisitionId:guid}")]
        public async Task<IActionResult> DeleteEventRequisition(Guid requisitionId)
        {
            await eventRequisitionService.DeleteEventRequisition(requisitionId);
            return NoContent();
        }

        //View Events Rquisitions History
        [HttpGet("eventRequisitionHistory/{memberId:guid}")]
        public async Task<IActionResult> ViewEventRequisitionHistory(Guid memberId)
        {
            var RequisitionList = await eventRequisitionService.GetEventRequisitionHistory(memberId);
            return Ok(RequisitionList);
        }

        // Handling Event Audits Endpoints ----------------------------------------------------
        // Create Audit of events
        //[HttpPost("createEventAudit")]
        //public async Task<IActionResult> CreateEventAudit([FromBody] CreateEventAuditDto eventAuditDto)
        //{

        //}
        // View audits of events
        // Update audits of events
        // Delete audits of events

        // Handling Yearly Events Requisitions Endpoints ---------------------------------------
        // Create yearly events requisitions
        [HttpPost("createYearlyBudgetRequisition/{chairpersonId:guid}")]
        public async Task<IActionResult> CreateYearlyBudget([FromBody] CreateYearlyBudgetDto newYearlyBudget, Guid chairpersonId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await yearlyBudgetService.CreateYearlyBudget(newYearlyBudget, chairpersonId);
            return Ok();
        }

        // View All yearly events budget
        [HttpGet("getAllYearlyBudgetRequisitions/{memberId:guid}")]
        public async Task<IActionResult> GetAllYearlyBudget(Guid memberId)
        {
            var yearlyBudgets = await yearlyBudgetService.GetAllYearlyBudgets(memberId);
            return Ok(yearlyBudgets);
        }

        // View yearly events requisitions
        // 


        // Handling Profiles Endpoints ---------------------------------------------------------
        // View President Profile
        [HttpGet("viewPresidentProfile/{chairpersonId:guid}")]
        public async Task<IActionResult> GetPresidentProfile(Guid chairpersonId)
        {
            var president = await memberService.GetMemberProfile(chairpersonId);
            return Ok(president);
        }

        // Edit President Profile
        [HttpPut("updatePresidentProfile/{presidentId:guid}")]
        public async Task<IActionResult> UpdatePresidentProfile(Guid presidentId, EditMemberProfileDto editMemberProfile)
        {
            await memberService.UpdateMemberProfile(presidentId, editMemberProfile);
            return Ok();
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

        
    }
}