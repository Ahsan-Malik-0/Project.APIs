using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Model;

namespace Project.APIs.Controllers.CRUD
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocietyController : ControllerBase
    {
        private readonly DB _dB;

        public SocietyController(DB dB) => _dB = dB;

        [HttpGet]
        public async Task<IActionResult> GetAllSocieties()
        {
            var societies = await _dB.Societies.ToListAsync();
            return Ok(societies);
        }

        [HttpGet("{id:guid}", Name = "GetSocietyById")]
        public async Task<IActionResult> GetSocietyById([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { Error = "Id must be a non-empty GUID." });
            }

            var society = await _dB.Societies.FindAsync(id);
            if (society == null)
            {
                return NotFound();
            }

            return Ok(society);
        }

        [HttpPost]
        public async Task<IActionResult> AddSociety([FromBody] SocietyDto newSociety)
        {
            if (newSociety is null)
            {
                return BadRequest(new { Error = "Request body is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var society = new Society
            {
                Name = newSociety.Name,
                Description = newSociety.Description
            };

            await _dB.Societies.AddAsync(society);
            await _dB.SaveChangesAsync();

            return CreatedAtRoute("GetSocietyById", new { id = society.Id }, society);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateSociety([FromRoute] Guid id, [FromBody] SocietyDto societyDto)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { Error = "Id must be a non-empty GUID." });
            }

            if (societyDto is null)
            {
                return BadRequest(new { Error = "Request body is required." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var society = await _dB.Societies.FindAsync(id);
            if (society is null)
            {
                return NotFound();
            }

            society.Name = societyDto.Name;
            society.Description = societyDto.Description;

            _dB.Societies.Update(society);
            await _dB.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSociety([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { Error = "Id must be a non-empty GUID" });
            }

            var society = await _dB.Societies.FindAsync(id);
            if (society is null)
            {
                return NotFound();
            }

            _dB.Societies.Remove(society);
            await _dB.SaveChangesAsync();
            return NoContent();
        }
    }
}
