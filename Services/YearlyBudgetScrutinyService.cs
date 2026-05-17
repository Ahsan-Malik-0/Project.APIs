using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Exceptions;
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
                    Id = ybs.Id,
                    AdministrationName = ybs.Name,
                    AdministrationComment = ybs.Comment,
                    AdministrationRole = ybs.Administration!.Role,
                    // AdministrationStatus = ybs.Status!,
                    CommentDate = ybs.Date,
                }).ToListAsync();
                
            if (details == null || details.Count == 0)
            {
                throw new NotFoundException("No scrutiny details found for the given yearly budget ID.");
            }

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
                    // Status = addComment.AdministrationStatus,
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

        public async Task DeleteComments(Guid commentId)
        {
            var commentsToDelete = await _dB.YearlyBudgetScrutinies
                .FirstOrDefaultAsync(ybs => ybs.Id == commentId);

            if (commentsToDelete == null)
            {
                throw new Exception("Comment not found.");
            }

            _dB.YearlyBudgetScrutinies.Remove(commentsToDelete);
            await _dB.SaveChangesAsync();
        }


    }
}
