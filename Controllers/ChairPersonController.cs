using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;

namespace Project.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChairPersonController(EventService _eventService) : ControllerBase
    {
        [HttpGet("pendingRequisitions")]

        //Create Requisition

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
