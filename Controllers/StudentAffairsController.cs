using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;

namespace Project.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAffairsController(EventRequisitionService eventRequisitionService, MemberService memberService) : ControllerBase
    {
        // Handle Event Requisition Endpoints --------------------------------------------------------
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

        [HttpGet("RejectEventRequisition/{requisitionId:guid}")]
        public async Task<IActionResult> RejectEventRequisition(Guid requisitionId, [FromBody] string rejectionMessage)
        {
            ReviewEventRequisitionDto reviewEventRequisitionDto = new ReviewEventRequisitionDto
            {
                Status = "D",
                ReviewMessage = rejectionMessage
            };
            await eventRequisitionService.ReviewEventRequisition(requisitionId, reviewEventRequisitionDto);
            return Ok();
        }

        [HttpGet("AcceptEventRequisition/{requisitionId:guid}")]
        public async Task<IActionResult> AcceptEventRequisition(Guid requisitionId, [FromBody] AcceptEventRequisitionDto acceptEventRequisitionDto)
        {
            await eventRequisitionService.AcceptEventRequisition(requisitionId, acceptEventRequisitionDto);
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

        // View Chairperson Profile
        [HttpGet("viewChairpersonProfile/{studentAffairsId:guid}")]
        public async Task<IActionResult> GetChairpersonProfile(Guid studentAffairsId)
        {
            var chairperson = await memberService.GetMemberProfile(studentAffairsId);
            return Ok(chairperson);
        }

        // Edit Chairperson Profile
        [HttpPut("updateChairpersonProfile/{chairpersonId:guid}")]
        public async Task<IActionResult> UpdateChairpersonProfile(Guid chairpersonId, EditMemberProfileDto editMemberProfile)
        {
            await memberService.UpdateMemberProfile(chairpersonId, editMemberProfile);
            return Ok();
        }

    }
}
