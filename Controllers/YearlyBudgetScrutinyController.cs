using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.APIs.Exceptions;
using Project.APIs.Model.DTOs;
using Project.APIs.Services;

namespace Project.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YearlyBudgetScrutinyController(YearlyBudgetScrutinyService scrutinyService, YearlyBudgetService yearlyBudgetService) : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult TestApi()
        {
            string name = "Ahsan Malik";
            return Ok(name);
        }

        [HttpGet("yearlyBudgetDetails")]
        public async Task<IActionResult> ViewYearlyBudgetDetails()
        {
            try
            {
                var details = await yearlyBudgetService.GetAllYearlyBudgets();
                return Ok(details);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
              
                return StatusCode(500, new { message = $"Something went wrong. Please try again later {ex}." });
            }
        }

        [HttpGet("yearlyBudgetDetailsById/{yearlyBudgetId}")]
        public async Task<IActionResult> ViewYearlyBudgetDetailsById(Guid yearlyBudgetId)
        {
            try
            {
                var details = await yearlyBudgetService.GetYearlyBudgetById(yearlyBudgetId);
                return Ok(details);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("scrutinyDetails/{yearlyBudgetId}")]
        public async Task<IActionResult> ViewScrutinyDetails(Guid yearlyBudgetId)
        {
            var details = await scrutinyService.ViewScrutinyDetails(yearlyBudgetId);
            return Ok(details);
        }

        [HttpPost("addComment/{administrationId}")]
        public async Task<IActionResult> AddComment(Guid administrationId, [FromBody] AddCommentDto addComment)
        {
            await scrutinyService.AddComment(administrationId, addComment);
            return Ok();
        }
        
        [HttpDelete("deleteComment/{commentId}")]
        public async Task<IActionResult> DeleteComments(Guid commentId)
        {
            await scrutinyService.DeleteComments(commentId);
            return Ok();
        }
    }
}
