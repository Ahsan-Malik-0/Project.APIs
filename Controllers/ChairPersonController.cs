using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;

namespace Project.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChairPersonController(EventService _eventService, EventRequisitionService eventRequisitionService, MemberService memberService) : ControllerBase
    {
        //[HttpGet("pendingRequisitions")]

        //Create Requisition
        [HttpPost("createRequisition")]
        public async Task<IActionResult> CreateRequisition([FromBody] CreateEventRequisitionDto requisitionDto)
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

        //Pending Requisitions

        //Get event Requirements

        //View Rquisitions History

        //View Profile

        //Edit Profile

        //Accept or reject event

        //Add an event
        [HttpPost("addEvent")]
        public async Task<IActionResult> AddEvent([FromBody] AddEventDto newEvent)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _eventService.AddEvent(newEvent, "accept");
            return Created();
        }
    }
}
