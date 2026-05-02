using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Services
{
    public class YearlyBudgetService(DB _dB)
    {
        // Get recent added yearly budget
        public async Task<YearlyBudget> GetRecentYearlyBudget(Guid memberId)
        {
            var societyId = await _dB.Members
                .Where(m => m.Id == memberId && m.Role == "chairperson")
                .Select(m => m.SocietyId)
                .FirstOrDefaultAsync();

            if (societyId == Guid.Empty)
                throw new NotFoundException("Chairperson not found or does not belong to any society.");

            var recentYearlyBudget = await _dB.YearlyBudgets
                .Include(yb => yb.YearlyEvents)!
                    .ThenInclude(ye => ye.YearlyEventRequirements)
                .Where(yb => yb.SocietyId == societyId)
                .OrderByDescending(yb => yb.RequestedDate)
                .FirstOrDefaultAsync();

            if (recentYearlyBudget == null)
                throw new NotFoundException("No yearly budget found for the member's society.");

            return recentYearlyBudget;
        }
        
        public async Task<List<YearlyBudget>> GetAllYearlyBudgets(Guid memberId)
        {
            var societyId = await _dB.Members
                .Where(m => m.Id == memberId && m.Role == "chairperson")
                .Select(m => m.SocietyId)
                .FirstOrDefaultAsync();

            if (societyId == Guid.Empty)
                throw new NotFoundException("Chairperson not found or does not belong to any society.");

            return await _dB.YearlyBudgets
                .Include(yb => yb.YearlyEvents)!
                    .ThenInclude(ye => ye.YearlyEventRequirements)
                .Where(yb => yb.SocietyId == societyId)
                .OrderByDescending(yb => yb.RequestedDate)
                .ToListAsync();
        }

        public async Task<List<YearlyBudget>> GetYearlyBudgetsBySocietyId(Guid societyId)
        {
            return await _dB.YearlyBudgets
                .Include(yb => yb.YearlyEvents)!
                    .ThenInclude(ye => ye.YearlyEventRequirements)
                .Where(yb => yb.SocietyId == societyId)
                .OrderByDescending(yb => yb.RequestedDate)
                .ToListAsync();
        }

        public async Task CreateYearlyBudget(CreateYearlyBudgetDto newYearlyBudget, Guid chairpersonId)
        {
            // Get society ID of the chairperson
            var societyId = await _dB.Members
                .Where(m => m.Id == chairpersonId && m.Role == "chairperson")
                .Select(m => m.SocietyId)
                .FirstOrDefaultAsync();

            if (societyId == Guid.Empty)
                throw new NotFoundException("Chairperson not found or does not belong to any society.");

            using var transaction = await _dB.Database.BeginTransactionAsync();
            try
            {
                YearlyBudget yearlyBudget = new YearlyBudget()
                {
                    Session = newYearlyBudget.Session,
                    RequestedAmount = newYearlyBudget.RequestedAmount,
                    RequestedDate = newYearlyBudget.RequestedDate,
                    SocietyId = societyId
                };

                await _dB.YearlyBudgets.AddAsync(yearlyBudget);
                await _dB.SaveChangesAsync();

                if (newYearlyBudget.YearlyEvents != null && newYearlyBudget.YearlyEvents.Any())
                {
                    foreach (var yearlyEventDto in newYearlyBudget.YearlyEvents)
                    {
                        YearlyEvent yearlyEvent = new YearlyEvent()
                        {
                            Name = yearlyEventDto.Name,
                            EstimateAmount = yearlyEventDto.EstimateAmount,
                            EstimateMonth = yearlyEventDto.EstimateMonth,
                            YearlyBudgetId = yearlyBudget.Id
                        };

                        await _dB.YearlyEvents.AddAsync(yearlyEvent);
                        await _dB.SaveChangesAsync();

                        if (yearlyEventDto.YearlyEventRequirements != null && yearlyEventDto.YearlyEventRequirements.Any())
                        {
                            foreach (var requirementDto in yearlyEventDto.YearlyEventRequirements)
                            {
                                YearlyEventRequirement requirement = new YearlyEventRequirement()
                                {
                                    Name = requirementDto.Name,
                                    EstimatePrice = requirementDto.EstimatePrice,
                                    YearlyEventId = yearlyEvent.Id
                                };

                                await _dB.YearlyEventRequirements.AddAsync(requirement);
                            }
                            await _dB.SaveChangesAsync();
                        }
                    }
                }
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException("Unable to save yearly budget. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}