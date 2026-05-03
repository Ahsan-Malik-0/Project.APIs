using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;

namespace Project.APIs.Controllers.CRUD
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController(EventRequisitionService eventRequisitionService, MemberService memberService) : ControllerBase
    {
        [HttpGet("ViewPendingRequisitions")]
        public async Task<IActionResult> ViewPendingRequisitions()
        {
            var pendingRequisitions = await eventRequisitionService.GetPendingEventRequisitions();
            // Implementation for viewing pending requisitions
            return Ok(pendingRequisitions);
        }

        [HttpGet("ViewRequisitionDetails/{requisitionId:guid}")]
        public async Task<IActionResult> ViewRequisitionDetails(Guid requisitionId)
        {
            var requisitionDetails = await eventRequisitionService.GetEventRequisitionDetails(requisitionId);
            return Ok(requisitionDetails);
        }

        [HttpGet("ReleasedEventRequisitionBudget/{requisitionId:guid}")]
        public async Task<IActionResult> ReleasedEventRequisitionBudget(Guid requisitionId)
        {
            ReviewEventRequisitionDto reviewEventRequisitionDto = new ReviewEventRequisitionDto
            {
                Status = "F",
                ReviewMessage = null
            };
            await eventRequisitionService.ReviewEventRequisition(requisitionId, reviewEventRequisitionDto);
            return Ok();
        }

        [HttpGet("AmountHandover/{requisitionId:guid}")]
        public async Task<IActionResult> AmountHandover(Guid requisitionId)
        {
            ReviewEventRequisitionDto reviewEventRequisitionDto = new ReviewEventRequisitionDto
            {
                Status = "G",
                ReviewMessage = null
            };
            await eventRequisitionService.ReviewEventRequisition(requisitionId, reviewEventRequisitionDto);
            return Ok();
        }

        // Handle Profile Endpoints --------------------------------------------------------
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
