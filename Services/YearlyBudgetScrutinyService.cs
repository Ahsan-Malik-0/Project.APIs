using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
                    AdministrationRole = ybs.Member!.Role,
                    // AdministrationStatus = ybs.Status!,
                    CommentDate = ybs.Date,
                }).ToListAsync();

            if (details == null || details.Count == 0)
            {
                throw new NotFoundException("No scrutiny details found for the given yearly budget ID.");
            }

            return details;
        }

        public async Task AddComment(Guid memberId, AddCommentDto addComment)
        {
            try
            {
                var member = await _dB.Members.FirstOrDefaultAsync(m => m.Id == memberId);
                YearlyBudgetScrutiny newYearlyBudgetScrutiny = new YearlyBudgetScrutiny()
                {
                    Name = member!.Name,
                    Comment = addComment.AdministrationComment,
                    // Status = addComment.AdministrationStatus,
                    Date = addComment.CommentDate,
                    MemberId = memberId,
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

        // Get all events happens after top early budget of that society approval
        //public async Task<List<GetYearlyBudgetDetailsWithEventsDto>> GetYearlyBudgetWithEvents()
        //{
        //    var details = await _dB.YearlyBudgets
        //        .Where(yb => yb.AllotedAmount > 0)
        //        .ToListAsync();

        //    if (details == null)
        //        throw new NotFoundException("No approved yearly budget found for the given society ID.");

        //    var earlyBudgetDetailsWithEvents = new List<GetYearlyBudgetDetailsWithEventsDto>();

        //    foreach (var detail in details)
        //    {
        //        var yearlyBudgetDetails = new GetYearlyBudgetDetailsWithEventsDto()
        //        {
        //            Session = detail.Session,
        //            AllotedAmount = detail.AllotedAmount,
        //            AllotedDate = detail.AllotedDate,
        //            Credits = detail.Credits,
        //            RequisitionDetails = await _dB.EventRequisitions
        //                .Where(er => er._event!.SocietyId == detail.SocietyId && er.AllocatedDate >= detail.AllotedDate)
        //                .Select(er => new ViewRequisitionRequestDetailsDto()
        //                {
        //                    Id = er.Id,
        //                    EventName = er._event!.Name,
        //                    SocietyName = er._event!.Society!.Name,
        //                    RequestedAmount = er.RequestAmount,
        //                    AllotedAmount = er.AllocatedAmount,
        //                    BiitContribution = er.BiitContribution,
        //                    EventDate = er._event!.Date,
        //                    RequestedDate = er.RequestedDate
        //                }).ToListAsync()
        //        };

        //        earlyBudgetDetailsWithEvents.Add(yearlyBudgetDetails);
        //    }

        //    return earlyBudgetDetailsWithEvents;
        //}
    }
}
