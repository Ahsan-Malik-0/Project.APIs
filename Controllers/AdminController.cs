using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;

namespace Project.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(EventRequisitionService eventRequisitionService, MemberService memberService) : ControllerBase
    {
        [HttpGet("AcceptEventRequisition/{requisitionId:guid}")]
        public async Task<IActionResult> AcceptEventRequisition(Guid requisitionId)
        {
            ReviewEventRequisitionDto reviewEventRequisitionDto = new ReviewEventRequisitionDto
            {   
                Status = "C",
                ReviewMessage = null
            };
            await eventRequisitionService.ReviewEventRequisition(requisitionId, reviewEventRequisitionDto);
            return Ok();
        }

        [HttpGet("RejectEventRequisition/{requisitionId:guid}")]
        public async Task<IActionResult> RejectEventRequisition(Guid requisitionId, [FromBody] string rejectionMessage)
        {
            ReviewEventRequisitionDto reviewEventRequisitionDto = new ReviewEventRequisitionDto
            {
                Status = "B",
                ReviewMessage = rejectionMessage
            };
            await eventRequisitionService.ReviewEventRequisition(requisitionId, reviewEventRequisitionDto);
            return Ok();
        }

        [HttpGet("ViewPendingRequisitions")]
        public async Task<IActionResult> ViewPendingRequisitions()
        {
            var pendingRequisitions = await eventRequisitionService.GetPendingEventRequisitions();
            return Ok(pendingRequisitions);
        }

        [HttpGet("ViewRequisitionDetails/{requisitionId:guid}")]
        public async Task<IActionResult> ViewRequisitionDetails(Guid requisitionId)
        {
            var requisitionDetails = await eventRequisitionService.GetEventRequisitionDetails(requisitionId);
            return Ok(requisitionDetails);
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
