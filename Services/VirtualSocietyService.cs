using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Services
{
    public class VirtualSocietyService(DB _dB, EventService eventService)
    {
        //public async Task CreateVirtualSociety(CreateVirtualSocietyDto newVirtualSociety)
        //{
        //    try
        //    {
        //        var member = await _dB.Members.FirstOrDefaultAsync(m => m.Id == newVirtualSociety.MemberId);
        //        if(member == null)
        //            throw new NotFoundException("Member not found");

        //        else if (member!.Role != "chairperson")
        //            throw new BusinessRuleException("Manager can not be other then chairperson");

        //        VirtualSociety virtualSociety = new VirtualSociety()
        //        {
        //            Name = newVirtualSociety.Name,
        //            Description = newVirtualSociety.Description ?? null,
        //            RegistrationEndDate = newVirtualSociety.RegistrationEndDate,
        //            MemberId = newVirtualSociety.MemberId,
        //        };

        //        await _dB.VirtualSocieties.AddAsync(virtualSociety);
        //        await _dB.SaveChangesAsync();

        //    }
        //    catch (DbUpdateException)
        //    {
        //        throw new BusinessRuleException("Unable to save requisition. Please try again.");
        //    }
        //    catch (Exception)
        //    {
        //        throw; // goes to 500 handler
        //    }
        //}




        public async Task CreateVirtualSociety(CreateVirtualSocietyDto newVirtualSociety)
        {

            using var transaction = await _dB.Database.BeginTransactionAsync();
            try
            {
                var member = await _dB.Members.FirstOrDefaultAsync(m => m.Id == newVirtualSociety.ChairpersonId);
                if (member == null)
                    throw new NotFoundException("Member not found");

                else if (member!.Role != "chairperson")
                    throw new BusinessRuleException("Manager can not be other then chairperson");

                VirtualSociety virtualSociety = new VirtualSociety()
                {
                    Name = newVirtualSociety.Name,
                    Description = newVirtualSociety.Description ?? null,
                    RegistrationEndDate = newVirtualSociety.RegistrationEndDate,
                    MemberId = newVirtualSociety.ChairpersonId,
                };

                await _dB.VirtualSocieties.AddAsync(virtualSociety);
                await _dB.SaveChangesAsync();

                if (newVirtualSociety.SocietyIds == null || newVirtualSociety.SocietyIds.Count == 0) throw new NotFoundException("Socities not found");

                foreach (var SocietyId in newVirtualSociety.SocietyIds)
                {

                    VirtualSocietyContribution virtualSocietyContribution = new VirtualSocietyContribution()
                    {
                        Contribution = 0,
                        VirtualSocietyId = virtualSociety.Id,
                        SocietyId = SocietyId
                    };

                    await _dB.VirtualSocietyContributions.AddAsync(virtualSocietyContribution);
                }


                await _dB.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch (DbUpdateException)
            {
                throw new BusinessRuleException("Unable to save requisition. Please try again.");
            }
            catch (Exception)
            {
                throw; // goes to 500 handler
            }
        }

        public async Task<List<GetVirtualSocietyDetailsDto>> GetVirtualSocietiesDetails()
        {
            var result = await _dB.VirtualSocieties
            .Select(vs => new
            {
                vs.Id, 
                vs.Name,
                vs.MemberId,
                vs.TotalContribution,
                vs.RegistrationEndDate,
                ContributedSocieties = _dB.VirtualSocietyContributions
                .Where(vsc => vsc.VirtualSocietyId == vs.Id)
                .Select(vsc => new
                {
                    vsc.Society!.Name,
                    vsc.Society!.Members!.FirstOrDefault(m => m.Role == "chairperson")!.Id,
                    vsc.Contribution
                }).ToList(),
                Events = _dB.Events
                        .Where(e => e.VirtualSocietyId == vs.Id)
                        .Include(e => e.Requirements)
                        .ToList(),
                Requisition = _dB.EventRequisitions
                        .FirstOrDefault(er => er.Events!.FirstOrDefault()!.VirtualSocietyId == vs.Id)
            })
            .AsNoTracking()
            .ToListAsync();

            if (result == null)
                throw new NotFoundException("No virtual society found");

            foreach(var item in result)
            {
                if(item.Requisition != null)
                {
                    item.Requisition.Status = StatusMap.GetValueOrDefault(item.Requisition.Status, "Unknown");
                }
            }

            var virtualSocieties = result
                .Select(r => new GetVirtualSocietyDetailsDto()
                { 
                    VirtualSocietyId = r.Id,
                    VirtualSocietyName = r.Name,
                    RegistrationEndDate = r.RegistrationEndDate,
                    ManagerId = r.MemberId,
                    TotalContribution = r.TotalContribution,
                    VirtualSocietyEvents = r.Events,
                    ContributedSocieties = r.ContributedSocieties
                    .Select(cs => new ContributedSocietiesDto
                    { 
                        SocietyName = cs.Name,
                        Chairpersonid = cs.Id,
                        Conrtibution = cs.Contribution,
                    }).ToList(),
                    VirtualSocietyRequisition = r.Requisition
                }).ToList();

            return virtualSocieties;

        }

        public async Task<List<GetVirtualSocietyDetailsForFinanceDto>> GetVirtualSocietiesDetailsForFinance()
        {
            // get those requisitions which status is equal to E along with their societies
            var requisitionIds = await _dB.Events.Where(e => e.SocietyId == null && e.RequisitionId != null).Select(e => e.RequisitionId).ToListAsync();

            if (requisitionIds == null)
                throw new NotFoundException("Requisitions not found");

            var requisitions = await _dB.EventRequisitions
                .Where(er => requisitionIds.Contains(er.Id) && er.Status == "E")
                .ToListAsync();

            if (requisitions == null)
                throw new NotFoundException("Requisitions not found");



            var result = await _dB.VirtualSocieties
            .Select(vs => new
            {
                vs.Id,
                vs.Name,
                vs.MemberId,
                vs.TotalContribution,
                vs.RegistrationEndDate,
                ContributedSocieties = _dB.VirtualSocietyContributions
                .Where(vsc => vsc.VirtualSocietyId == vs.Id)
                .Select(vsc => new
                {
                    vsc.Society!.Name,
                    vsc.Society!.Members!.FirstOrDefault(m => m.Role == "chairperson")!.Id,
                    vsc.Contribution
                }).ToList(),
            })
            .AsNoTracking()
            .ToListAsync();

            if (result == null)
                throw new NotFoundException("No virtual society found");


            var virtualSocieties = result
                .Select(r => new GetVirtualSocietyDetailsForFinanceDto()
                {
                    VirtualSocietyId = r.Id,
                    VirtualSocietyName = r.Name,
                    TotalContribution = r.TotalContribution,
                    ContributedSocieties = r.ContributedSocieties
                    .Select(cs => new ContributedSocietiesDto
                    {
                        SocietyName = cs.Name,
                        Chairpersonid = cs.Id,
                        Conrtibution = cs.Contribution,
                    }).ToList()
                }).ToList();

            return virtualSocieties;

        }

        public async Task<List<Event>> GetVirtualSocietyEvents(Guid virtualSocietyId)
        {
            var events = await _dB.Events
                .Include(e => e.Requirements)
                .Where(e => e.VirtualSocietyId == virtualSocietyId)
                .ToListAsync();

            if (events == null) throw new NotFoundException("Events not found");

            return events;
        }

        //public async Task<List<GetPastVirtualSocietyDetailsDto>> GetPastVirtualSocietyDetails()
        //{
        //    var pastVirtalSocieties = await _dB.VirtualSocieties
        //                                        .Where(vs => vs.RegistrationEndDate <= DateTime.Today.Date)
        //                                        .ToListAsync();

        //    if(pastVirtalSocieties.Count == 0)
        //        throw new NotFoundException("Virtual Societies not found");

        //    return pastVirtalSocieties.Select(pvs => new GetPastVirtualSocietyDetailsDto()
        //    { 
        //        Name = pvs.Name,
        //        RegistrationEndDate = pvs.RegistrationEndDate,
        //        MemberId= pvs.MemberId
        //    })
        //    .ToList();
        //}

        //public async Task<List<GetRecentVirtualSocietyDetailsDto>> GetRecentVirtualSocietyDetails()
        //{
        //    var pastVirtalSocieties = await _dB.VirtualSocieties
        //                                        .Where(vs => vs.RegistrationEndDate > DateTime.Now)
        //                                        .ToListAsync();

        //    if (pastVirtalSocieties.Count == 0)
        //        throw new NotFoundException("Virtual Societies not found");

        //    return pastVirtalSocieties.Select(pvs => new GetRecentVirtualSocietyDetailsDto()
        //    {
        //        Id = pvs.Id,
        //        Name = pvs.Name,
        //        RegistrationEndDate = pvs.RegistrationEndDate,
        //        MemberId = pvs.MemberId
        //    })
        //    .ToList();
        //}

        public async Task ContributeToVirtualSociety(Guid memberId, ContributeToVirtualSocietyDto contributeToVC)
        {
            using var transaction = await _dB.Database.BeginTransactionAsync();
            try
            {
                var societyId = await _dB.Members
                    .Where(m => m.Id == memberId)
                    .Select(m => m.SocietyId)
                    .FirstOrDefaultAsync();

                if (societyId == null)
                    throw new NotFoundException("Member Not Found");

                Guid societyIdValue = societyId.Value;

                // Deduction of contribution amount from yearly budget 
                var yearlyBudget = await _dB.YearlyBudgets.FirstOrDefaultAsync(yb => yb.SocietyId == societyIdValue);

                if (yearlyBudget == null)
                    throw new NotFoundException("Yearly Budget Not Found");
                else if (yearlyBudget.AllotedAmount == 0)
                    throw new BusinessRuleException("Yearly Budget Not Alloted Yet");
                else if (yearlyBudget.Credits == 0)
                    throw new BusinessRuleException("No society budget remaining");
                else if ((yearlyBudget.Credits - contributeToVC.Contribution) < 0)
                    throw new BusinessRuleException("Insufficient society budget");

                yearlyBudget.Credits -= contributeToVC.Contribution;

                // Updatin total contibution of Virtual society by adding contribution of society
                var vertialSociety = await _dB.VirtualSocieties.FirstOrDefaultAsync(vs => vs.Id == contributeToVC.VirtualSocietyId);
                if (vertialSociety == null)
                    throw new NotFoundException("Virtual Societiy not found");

                vertialSociety.TotalContribution += contributeToVC.Contribution;

                // Add row in VirtualSocietyContributions table
                VirtualSocietyContribution virtualSocietyContribution = new VirtualSocietyContribution()
                {
                    Contribution = contributeToVC.Contribution,
                    VirtualSocietyId = contributeToVC.VirtualSocietyId,
                    SocietyId = societyIdValue
                };

                await _dB.VirtualSocietyContributions.AddAsync(virtualSocietyContribution);

                await _dB.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException("Unable to save requisition. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // goes to 500 handler
            }
        }

        public async Task CreateVirtualSocietyEvents(AddEventDto newEvent)
        {
            await eventService.AddEvent(newEvent, "vs");
        }

        public async Task<List<ChairpersonDetailsForVirtualSocietyDto>> GetChairpersonsListForVS()
        {
            var result = await _dB.Members
                .Where(m => m.Role == "chairperson")
                .Select(m => new
                {
                    m.Id,
                    m.Name
                })
                .AsNoTracking()
                .ToListAsync();

            if (result.Count == 0)
                throw new NotFoundException("Chairpersons not found");

            var chairpersonList = result.Select
                (r => new ChairpersonDetailsForVirtualSocietyDto
                {
                    ChairperonId = r.Id,
                    ChairperonName = r.Name,
                }).ToList();

            return chairpersonList;
        }


        public async Task CreateVirtualSocietyRequisition(CreateVirtualSocietyRequisitionDto newRequisition)
        {
            using var transaction = await _dB.Database.BeginTransactionAsync();
            try
            {

                EventRequisition eventRequisition = new EventRequisition()
                {
                    Subject = newRequisition.Subject,
                    Body = newRequisition.Body,
                    RequestedDate = newRequisition.RequestedDate,
                    RequestAmount = newRequisition.RequestedAmount,
                    Status = "A",
                    Events = null!
                };

                await _dB.EventRequisitions.AddAsync(eventRequisition);
                await _dB.SaveChangesAsync();

                foreach (Guid eventId in newRequisition.EventIds!)
                {
                    var _event = await _dB.Events.FirstOrDefaultAsync(e => e.Id == eventId);
                    if (_event == null)
                    {
                        throw new NotFoundException("Event Not Found");
                    }

                    _event.Status = "accepted";
                    _event.RequisitionId = eventRequisition.Id;

                }

                await _dB.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException("Unable to save requisition. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // goes to 500 handler
            }
        }

        public async Task UpdateEventRequisition(Guid requisitionId, UpdateVirtualEventRequisitionDto updateEventRequisition)
        {
            var transaction = await _dB.Database.BeginTransactionAsync();
            try
            {

                var requisition = await _dB.EventRequisitions.FindAsync(requisitionId);

                if (requisition == null)
                    throw new NotFoundException("Requisition Not Found");

                requisition.Subject = updateEventRequisition.Subject;
                requisition.Body = updateEventRequisition.Body;
                requisition.RequestedDate = updateEventRequisition.RequestedDate;
                requisition.RequestAmount = updateEventRequisition.RequestedAmount;

                if (requisition.Status == "B") requisition.Status = "A";
                if (requisition.Status == "D") requisition.Status = "C";
                if (requisition.Status == "F") requisition.Status = "E";

                // Remove requisition id from old events
                var oldEventList = await _dB.Events.Where(e => e.RequisitionId == requisitionId).ToListAsync();

                foreach (var _event in oldEventList)
                {
                    _event.RequisitionId = null;
                    _event.Status = "pending";
                }

                // Add requisition id in new events
                foreach (Guid eventId in updateEventRequisition.EventIds)
                {
                    var _event = await _dB.Events.FirstOrDefaultAsync(e => e.Id == eventId);
                    if (_event == null)
                    {
                        throw new NotFoundException("Event Not Found");
                    }

                    _event.Status = "accepted";
                    _event.RequisitionId = requisitionId;
                }

                _dB.EventRequisitions.Update(requisition);
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

        public async Task<List<VirtualRequisitionDetailsForChairperson>> GetRequisitionDetailsForChairperson(Guid memberId)
        {
            var result = await _dB.EventRequisitions
                .Where(er => er.Status != "H" && er.Status != "I" && er.Status != "J")
                .Where(er => er.Events!.FirstOrDefault()!.Society!.Members!.Any(m => m.Id == memberId))
                .Select(er => new
                {
                    er.Id,
                    er.RequestedDate,
                    er.RequestAmount,
                    er.Status,
                    er.ReviewMessage,
                    er.Subject,
                    er.Body,
                    Events = er.Events!.Select(e => new
                    {
                        e.Id,
                        e.Name,
                        Requirements = e.Requirements.ToList(),
                        Society = new
                        {
                            e.Society!.Id,
                            e.Society.Name,
                            Member = e.Society.Members!.FirstOrDefault(m => m.Id == memberId)!.Name
                        }
                    }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            string member = result!.FirstOrDefault()!.Events!.FirstOrDefault()!.Society.Member;

            return result.Select(er => new VirtualRequisitionDetailsForChairperson()
            {
                Id = er.Id,
                RequestedDate = er.RequestedDate,
                RequestedAmount = er.RequestAmount,
                Status = StatusMap.GetValueOrDefault(er.Status, "Unknown"),
                ReviewMessage = er.ReviewMessage,
                Subject = er.Subject,
                Body = er.Body,
                ChairpersonName = member,
                Events = er.Events.Select(e => new Event
                {
                    Id = e.Id,
                    Name = e.Name,
                    Status = null!,
                    Requirements = e.Requirements,
                    Society = new Society
                    {
                        Id = e.Society.Id,
                        Name = e.Society.Name,
                        Members = new List<Member>
                        {
                            new Member { Name = e.Society.Member, Username = null!, Role = null! }
                        }
                    }
                }).ToList()
            }).ToList();
        }

        private static readonly Dictionary<string, string> StatusMap = new()
        {
            ["A"] = "Pending",
            ["B"] = "Reject By Student Affairs",
            ["C"] = "Approved By Student Affairs",
            ["D"] = "Reject By Admin",
            ["E"] = "Approved By Admin",
            ["F"] = "Reject By Finance",
            ["G"] = "Budget Released By Finance",
            ["H"] = "Event Completed",
            ["I"] = "Request For Audit",
            ["J"] = "Audit Cleared"
        };
    }
}