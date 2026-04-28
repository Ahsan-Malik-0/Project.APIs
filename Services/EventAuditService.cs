using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Services
{
    public class EventAuditService(DB _dB)
    {

        public async Task CreateEventAudit(CreateEventAuditDto newEventAudit)
        {
            using var transaction = await _dB.Database.BeginTransactionAsync();
            try
            {

                EventAudit eventAudit = new EventAudit()
                {
                    FundProvided = newEventAudit.FundProvided,
                    SpendAmount = newEventAudit.SpendAmount,
                    RevenueGenerated = newEventAudit.RevenueGenerated,
                    RemainingAmount = newEventAudit.RemainingAmount,
                    EventId = newEventAudit.EventId,
                };

                if (newEventAudit.RemainingAmount > 0) eventAudit.Status = "take";
                else if (newEventAudit.RemainingAmount < 0) eventAudit.Status = "give";
                if (newEventAudit.RemainingAmount == 0) eventAudit.Status = "clear";

                await _dB.EventAudits.AddAsync(eventAudit);
                await _dB.SaveChangesAsync();

                foreach (var spend in newEventAudit.Spends)
                {
                    AuditSpend auditSpend = new AuditSpend()
                    {
                        Vender = spend.VenderName,
                        Description = spend.ItemDescription,
                        Amount = spend.Amount,
                        ReceiptPiture = spend.ReceiptPiture,
                        AuditId = eventAudit.Id,
                    };

                    await _dB.AuditSpends.AddAsync(auditSpend);
                }

                await _dB.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException("Unable to save event. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // goes to 500 handler
            }
        }

        public async Task UpdateEventAudit(Guid eventAuditId, UpdateEventAuditDto updatedEventAudit)
        {
            using var transaction = await _dB.Database.BeginTransactionAsync();
            try
            {
                var existingEventAudit = await _dB.EventAudits.FirstOrDefaultAsync(ea => ea.EventId == eventAuditId);

                if (existingEventAudit == null) throw new NotFoundException("Event audit not found");

                // Update Existing Event Audit
                existingEventAudit.FundProvided = updatedEventAudit.FundProvided;
                existingEventAudit.SpendAmount = updatedEventAudit.SpendAmount;
                existingEventAudit.RevenueGenerated = updatedEventAudit.RevenueGenerated;
                existingEventAudit.RemainingAmount = updatedEventAudit.RemainingAmount;

                if (updatedEventAudit.RemainingAmount > 0) existingEventAudit.Status = "take";
                else if (updatedEventAudit.RemainingAmount < 0) existingEventAudit.Status = "give";
                if (updatedEventAudit.RemainingAmount == 0) existingEventAudit.Status = "clear";

                // Delete all past spends of event audit
                if (existingEventAudit.Spends!.Any())
                {
                    _dB.AuditSpends.RemoveRange(existingEventAudit.Spends!);
                }

                // Add new spends of event audit
                foreach (var spend in updatedEventAudit.Spends)
                {
                    AuditSpend auditSpend = new AuditSpend()
                    {
                        Vender = spend.VenderName,
                        Description = spend.ItemDescription,
                        Amount = spend.Amount,
                        ReceiptPiture = spend.ReceiptPiture,
                        AuditId = existingEventAudit.Id,
                    };

                    await _dB.AuditSpends.AddAsync(auditSpend);
                }

                await _dB.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException("Unable to update event. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
