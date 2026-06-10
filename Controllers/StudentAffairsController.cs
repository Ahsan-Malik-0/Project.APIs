using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;
using static Project.APIs.Model.DTOs.AdministrationDto;

namespace Project.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAffairsController(EventRequisitionService eventRequisitionService, MemberService memberService, AdministrationService administrationService) : ControllerBase
    {
        //// Handle Event Requisition Endpoints --------------------------------------------------------
        //[HttpGet("ViewPendingRequisitions")]
        //public async Task<IActionResult> ViewPendingRequisitions()
        //{
        //    var pendingRequisitions = await eventRequisitionService.GetPendingEventRequisitions('A');
        //    // Implementation for viewing pending requisitions
        //    return Ok(pendingRequisitions);
        //}

        //[HttpGet("ViewRequisitionDetails/{requisitionId:guid}")]
        //public async Task<IActionResult> ViewRequisitionDetails(Guid requisitionId)
        //{
        //    var requisitionDetails = await eventRequisitionService.GetEventRequisitionDetails(requisitionId);
        //    return Ok(requisitionDetails);
        //}

        //[HttpGet("ViewApprovedRequisitionDetails")]
        //public async Task<IActionResult> ViewAcceptedEventDetails()
        //{
        //    var requisitionDetails = await eventRequisitionService.ViewRequisitionDetailsForStudentAffairs();
        //    return Ok(requisitionDetails);
        //}

        //[HttpPost("RejectEventRequisition/{requisitionId:guid}")]
        //public async Task<IActionResult> RejectEventRequisition(Guid requisitionId, [FromBody] ResponseMessageDto responseMessage)
        //{
        //    ReviewEventRequisitionDto reviewEventRequisitionDto = new ReviewEventRequisitionDto
        //    {
        //        Status = "B",
        //        ReviewMessage = responseMessage.ResponseMessage
        //    };
        //    await eventRequisitionService.ReviewEventRequisition(requisitionId, reviewEventRequisitionDto);
        //    return Ok();
        //}

        //[HttpPost("ApproveEventRequisition/{requisitionId:guid}")]
        //public async Task<IActionResult> ApproveEventRequisition(Guid requisitionId, [FromBody] ApproveEventRequisitionDto acceptEventRequisitionDto)
        //{
        //    await eventRequisitionService.ApproveEventRequisition(requisitionId, acceptEventRequisitionDto);
        //    return Ok();
        //}

        //[HttpGet("MarkEventAsCompleted/{requisitionId:guid}")]
        //public async Task<IActionResult> EventCompleted(Guid requisitionId)
        //{
        //    ReviewEventRequisitionDto reviewEventRequisitionDto = new ReviewEventRequisitionDto
        //    {
        //        Status = "H",
        //        ReviewMessage = "Event Completed"
        //    };
        //    await eventRequisitionService.ReviewEventRequisition(requisitionId, reviewEventRequisitionDto);
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
