using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Controllers.CRUD
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController(DB _dB) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _dB.Events
                .Include(e => e.Society)
                .ToListAsync();
            return Ok(events);
        }

        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] EventDto eventDto)
        {
            var society = _dB.Societies.FirstOrDefaultAsync(s => s.Id == eventDto.SocietyId);

            if (society == null)
            {
                return BadRequest("Society Not Found");
            }

            Event _event = new Event()
            {
                Name = eventDto.Name,
                Date = eventDto.Date,
                Status = eventDto.Status!,
                Message = eventDto.Message,
                SocietyId = eventDto.SocietyId
            };

            await _dB.Events.AddAsync(_event);
            await _dB.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var _event = await _dB.Events.FirstOrDefaultAsync(e => e.Id == id);

            if (_event == null)
                return BadRequest("Event Not Found");

            _dB.Remove(_event);
            await _dB.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateEvent([FromBody] EventDto eventDto, Guid id)
        {
            var _event = await _dB.Events.FirstOrDefaultAsync(e => e.Id == id);

            if (_event == null)
                return BadRequest("Event Not Found");

            _event.Name = eventDto.Name;
            _event.Date = eventDto.Date;
            _event.Status = eventDto.Status!;
            _event.Message = eventDto.Message;

            _dB.Events.Update(_event);
            await _dB.SaveChangesAsync();

            return Ok();
        }

    }
}
