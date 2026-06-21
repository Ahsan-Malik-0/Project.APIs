using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Services
{
    public class EventAuditService(DB _dB, EventRequisitionService eventRequisitionService)
    {
        public async Task<EventAudit> GetEventAuditById(Guid eventId)
        {
            var eventAudit = await _dB.EventAudits.Include(ea => ea.Spends).FirstOrDefaultAsync(ea => ea.EventId == eventId);

            if (eventAudit == null) throw new NotFoundException("Event audit not found");

            return eventAudit;
        }

        public async Task CreateEventAudit(Guid eventId, CreateEventAuditDto newEventAudit)
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
                    EventId = eventId,
                };

                if (newEventAudit.RemainingAmount > 0) eventAudit.Status = "give";
                else if (newEventAudit.RemainingAmount < 0) eventAudit.Status = "take";
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
                        ReceiptPicture = spend.ReceiptPicture ?? null,
                        EventAuditId = eventAudit.Id,
                    };

                    await _dB.AuditSpends.AddAsync(auditSpend);
                }

                await _dB.SaveChangesAsync();
                await transaction.CommitAsync();
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
                var existingEventAudit = await _dB.EventAudits
                    .Include(ea => ea.Spends)
                    .FirstOrDefaultAsync(ea => ea.Id == eventAuditId);

                if (existingEventAudit == null) throw new NotFoundException("Event audit not found");

                // Update Existing Event Audit
                existingEventAudit.FundProvided = updatedEventAudit.FundProvided;
                existingEventAudit.SpendAmount = updatedEventAudit.SpendAmount;
                existingEventAudit.RevenueGenerated = updatedEventAudit.RevenueGenerated;
                existingEventAudit.RemainingAmount = updatedEventAudit.RemainingAmount;

                if (updatedEventAudit.RemainingAmount > 0) existingEventAudit.Status = "give";
                else if (updatedEventAudit.RemainingAmount < 0) existingEventAudit.Status = "take";
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
                        ReceiptPicture = spend.ReceiptPicture,
                        EventAuditId = existingEventAudit.Id,
                    };

                    await _dB.AuditSpends.AddAsync(auditSpend);
                }

                await _dB.SaveChangesAsync();
                await transaction.CommitAsync();
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

        public async Task DeleteEventAuditById(Guid eventAuditId)
        {
            var transaction = await _dB.Database.BeginTransactionAsync();
            try
            {
                var existingEventAudit = await _dB.EventAudits.Include(ea => ea.Spends).FirstOrDefaultAsync(ea => ea.Id == eventAuditId);

                if (existingEventAudit == null) throw new NotFoundException("Event audit not found");

                if (existingEventAudit.Spends != null && existingEventAudit.Spends.Any())
                {
                    _dB.AuditSpends.RemoveRange(existingEventAudit.Spends);
                }

                _dB.EventAudits.Remove(existingEventAudit);
                await _dB.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException("Unable to delete event audit. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //public async Task UpdateAuditStatus(Guid eventAuditId, string status)
        //{
        //    var transaction = await _dB.Database.BeginTransactionAsync();
        //    try
        //    {
        //        if (status == "clear")
        //        {
        //            var requisition = await _dB.EventRequisitions.FirstOrDefaultAsync(er => er.EventId == _dB.EventAudits.FirstOrDefault(ea => ea.Id == eventAuditId)!.EventId);

        //            await eventRequisitionService.UpdateRequisitionStatus(requisition!.Id);
        //        }
                
        //        var existingEventAudit = await _dB.EventAudits.FirstOrDefaultAsync(ea => ea.Id == eventAuditId);

        //        if (existingEventAudit == null) throw new NotFoundException("Event audit not found");

        //        existingEventAudit.Status = status;
        //        await _dB.SaveChangesAsync();

        //        await transaction.CommitAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        await transaction.RollbackAsync();
        //        throw new BusinessRuleException("Unable to update event audit status. Please try again.");
        //    }
        //}
    }
}
