using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Services
{
    public class YearlyBudgetScrutinyService(DB _dB)
    {
        public async Task<List<ViewScrutinyDetailsDto>> ViewScrutinyDetails(Guid yearlyBudgetId)
        {
            var details = await _dB.YearlyBudgetScrutinies
                .Where(ybs => ybs.YearlyBudgetId == yearlyBudgetId)
                .Select(ybs => new ViewScrutinyDetailsDto()
                { 
                    AdministrationName = ybs.Name,
                    AdministrationComment = ybs.Comment,
                    AdministrationStatus = ybs.Status!,
                    CommentDate = ybs.Date,
                }).ToListAsync();

            return details;
        }

        public async Task AddComment(Guid administrationId, AddCommentDto addComment)
        {
            try
            {
                YearlyBudgetScrutiny newYearlyBudgetScrutiny = new YearlyBudgetScrutiny()
                {
                    Name = addComment.AdministrationName,
                    Comment = addComment.AdministrationComment,
                    Status = addComment.AdministrationStatus,
                    AdministrationId = administrationId,
                    YearlyBudgetId = addComment.YearlyBudgetId
                };

                await _dB.YearlyBudgetScrutinies.AddAsync(newYearlyBudgetScrutiny);
                await _dB.SaveChangesAsync();
            }
            catch
            {
                throw;
            }

        }
    }
}
