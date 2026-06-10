using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;
using static Project.APIs.Model.DTOs.AdministrationDto;

namespace Project.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(EventRequisitionService eventRequisitionService, AdministrationService administrationService, EventService eventService) : ControllerBase
    {
        //[HttpGet("ApproveEventRequisition/{requisitionId:guid}")]
        //public async Task<IActionResult> ApproveEventRequisition(Guid requisitionId)
        //{
        //    ReviewEventRequisitionDto reviewEventRequisitionDto = new ReviewEventRequisitionDto
        //    {   
        //        Status = "E",
        //        ReviewMessage = null
        //    };
        //    await eventRequisitionService.ReviewEventRequisition(requisitionId, reviewEventRequisitionDto);
        //    return Ok();
        //}

        //[HttpPost("RejectEventRequisition/{requisitionId:guid}")]
        //public async Task<IActionResult> RejectEventRequisition(Guid requisitionId, [FromBody] ResponseMessageDto responseMessage)
        //{
        //    ReviewEventRequisitionDto reviewEventRequisitionDto = new ReviewEventRequisitionDto
        //    {
        //        Status = "D",
        //        ReviewMessage = responseMessage.ResponseMessage
        //    };
        //    await eventRequisitionService.ReviewEventRequisition(requisitionId, reviewEventRequisitionDto);
        //    return Ok();
        //}

        //[HttpGet("ViewPendingRequisitions")]
        //public async Task<IActionResult> ViewPendingRequisitions()
        //{
        //    var pendingRequisitions = await eventRequisitionService.GetPendingEventRequisitions('C');
        //    return Ok(pendingRequisitions);
        //}

        //[HttpGet("ViewRequisitionDetails/{requisitionId:guid}")]
        //public async Task<IActionResult> ViewRequisitionDetails(Guid requisitionId)
        //{
        //    var requisitionDetails = await eventRequisitionService.GetEventRequisitionDetails(requisitionId);
        //    return Ok(requisitionDetails);
        //}

        //[HttpGet("ViewReservedNonFinancialRequirements")]
        //public async Task<IActionResult> ViewEventRequirements()
        //{
        //    var requirements = await eventService.GetReservedNonFinancialRequirements();
        //    return Ok(requirements);
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
