using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;
using static Project.APIs.Model.DTOs.AdministrationDto;

namespace Project.APIs.Controllers.CRUD
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController(EventRequisitionService eventRequisitionService, AdministrationService administrationService) : ControllerBase
    {
        [HttpGet("ViewEventRequisitionDetails")]
        public async Task<IActionResult> ViewPendingRequisitions()
        {
            var eventRequisitions = await eventRequisitionService.ViewRequisitionDetailsForFinance();
            // Implementation for viewing pending requisitions
            return Ok(eventRequisitions);
        }

        //[HttpGet("ViewRequisitionDetails/{requisitionId:guid}")]
        //public async Task<IActionResult> ViewRequisitionDetails(Guid requisitionId)
        //{
        //    var requisitionDetails = await eventRequisitionService.GetEventRequisitionDetails(requisitionId);
        //    return Ok(requisitionDetails);
        //}

        [HttpPost("ReleasedEventRequisitionBudget/{requisitionId:guid}")]
        public async Task<IActionResult> ReleasedEventRequisitionBudget(Guid requisitionId, string reviewMessage)
        {
            ReviewEventRequisitionDto reviewEventRequisitionDto = new ReviewEventRequisitionDto
            {
                Status = "F",
                ReviewMessage = reviewMessage
            };
            await eventRequisitionService.ReviewEventRequisition(requisitionId, reviewEventRequisitionDto);
            return Ok();
        }


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
