using Microsoft.AspNetCore.Mvc;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;
using static Project.APIs.Model.DTOs.AdministrationDto;

namespace Project.APIs.Controllers.CRUD
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController(EventRequisitionService eventRequisitionService, AdministrationService administrationService, MemberService memberService, EventAuditService eventAuditService) : ControllerBase
    {
        //[HttpGet("ViewEventRequisitionDetails")]
        //public async Task<IActionResult> ViewPendingRequisitions()
        //{
        //    var eventRequisitions = await eventRequisitionService.ViewRequisitionDetailsForFinance();
        //    // Implementation for viewing pending requisitions
        //    return Ok(eventRequisitions);
        //}

        ////[HttpGet("ViewRequisitionDetails/{requisitionId:guid}")]
        ////public async Task<IActionResult> ViewRequisitionDetails(Guid requisitionId)
        ////{
        ////    var requisitionDetails = await eventRequisitionService.GetEventRequisitionDetails(requisitionId);
        ////    return Ok(requisitionDetails);
        ////}

        //[HttpPost("RejectEventRequisition/{requisitionId:guid}")]
        //public async Task<IActionResult> RejectEventRequisition(Guid requisitionId, [FromBody] ResponseMessageDto responseMessage)
        //{
        //    ReviewEventRequisitionDto reviewEventRequisitionDto = new ReviewEventRequisitionDto
        //    {
        //        Status = "F",
        //        ReviewMessage = responseMessage.ResponseMessage
        //    };
        //    await eventRequisitionService.ReviewEventRequisition(requisitionId, reviewEventRequisitionDto);
        //    return Ok();
        //}

        //[HttpGet("getChairpersonDetails/{societyName}")]
        //public async Task<IActionResult> GetChaipersonDetailsForRequisition(string societyName)
        //{
        //    var chairpersonDetails = await memberService.GetChairpersonDetailsForFinance(societyName);
        //    return Ok(chairpersonDetails);
        //}

        //[HttpPost("ReleasedEventRequisitionBudget/{requisitionId:guid}")]
        //public async Task<IActionResult> ReleasedEventRequisitionBudget(Guid requisitionId, [FromBody] ResponseMessageDto responseMessage)
        //{
        //    ReviewEventRequisitionDto reviewEventRequisitionDto = new ReviewEventRequisitionDto
        //    {
        //        Status = "G",
        //        ReviewMessage = responseMessage.ResponseMessage
        //    };
        //    await eventRequisitionService.ReviewEventRequisition(requisitionId, reviewEventRequisitionDto);
        //    return Ok();
        //}

        //[HttpGet("ViewEventRequisitionHistory")]
        //public async Task<IActionResult> ViewEventRequisitionHistory()
        //{
        //    var eventRequisitionHistory = await eventRequisitionService.ViewRequisitionDetailsForFinanceHistory();
        //    return Ok(eventRequisitionHistory);
        //}


        //// Handle audit endpoints --------------------------------------------------------
        //[HttpGet("RequestForEventAudit/{requisitionId:guid}")]
        //public async Task<IActionResult> RequestForEventAudit(Guid requisitionId)
        //{
        //    ReviewEventRequisitionDto reviewEventRequisitionDto = new ReviewEventRequisitionDto
        //    {
        //        Status = "I",
        //    };
        //    await eventRequisitionService.ReviewEventRequisition(requisitionId, reviewEventRequisitionDto);
        //    return Ok();
        //}

        [HttpGet("ViewEventAuditDetails/{eventId:guid}")]
        public async Task<IActionResult> ViewEventAuditDetails(Guid eventId)
        {
            var eventAuditDetails = await eventAuditService.GetEventAuditById(eventId);
            return Ok(eventAuditDetails);
        }

        //[HttpGet("verifyTakeAmount/{auditId:guid}")]
        //public async Task<IActionResult> UpdateEventAuditStatus(Guid auditId)
        //{
        //    string status = "clear";

        //    await eventAuditService.UpdateAuditStatus(auditId, status);
        //    return Ok();
        //}


        // Handle Profile Endpoints --------------------------------------------------------
        //View Profile
        [HttpGet("viewProfile/{memberId:guid}")]
        public async Task<IActionResult> GetProfile(Guid memberId)
        {
            var member = await administrationService.ViewProfile(memberId);
            return Ok(member);
        }

        //Edit Profile
        [HttpPut("updateProfile/{memberId:guid}")]
        public async Task<IActionResult> UpdateProfile(Guid memberId, [FromBody] UpdateAdminProfileDto updateAdminProfileDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Fill the credentials");

            await administrationService.EditProfile(memberId, updateAdminProfileDto);
            return Ok();
        }
    }
}
