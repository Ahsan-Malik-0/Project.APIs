using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Services
{
    public class EventRequisitionService(DB _dB)
    {

        // Event Requisition Status List
        // A -> Pending
        // B -> Rejected by Admin (Message)
        // C -> Accepted By Admin
        // D -> Rejected By SA (Message)
        // E -> Accepted By SA
        // F -> Budget Released By Finance
        // G -> Chairperson received the budget

        public async Task CreateEventRequisition(CreateEventRequisitionDto newRequisition)
        {
            using var transaction = await _dB.Database.BeginTransactionAsync();

            try
            {
                EventRequisition eventRequisition = new EventRequisition()
                {
                    RequestedDate = newRequisition.RequestedDate,
                    Subject = newRequisition.Subject,
                    Body = newRequisition.Body,
                    //EventId = newRequisition.EventId,
                    RequestAmount = newRequisition.RequestedAmount,
                    Status = "A",
                    Events = null
                };

                await _dB.EventRequisitions.AddAsync(eventRequisition);
                await _dB.SaveChangesAsync();

                var _event = await _dB.Events.FirstOrDefaultAsync(e => e.Id == newRequisition.EventId);
                if (_event == null)
                {
                    throw new NotFoundException("Event Not Found");
                }

                _event.Status = "accepted";
                _event.RequisitionId = eventRequisition.Id;

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

        public async Task<RequisitionDetailsForChairperson> GetEventRequisitionById(Guid requisitionId)
        {
            var result = await _dB.EventRequisitions
                .Where(er => er.Id == requisitionId)
                .Select(er => new
                {
                    er.Id,
                    er.RequestedDate,
                    er.RequestAmount,
                    er.Status,
                    er.ReviewMessage,
                    er.Subject,
                    er.Body,
                    Event = er.Events!.Select(e => new
                    {
                        e.Id,
                        e.Name,
                        e.EventDate,
                        e.StartTime,
                        e.EndTime,
                        Requirements = e.Requirements.ToList(),
                        Society = new
                        {
                            e.Society!.Id,
                            e.Society.Name,
                            Member = e.Society.Members!.FirstOrDefault(m => m.SocietyId == e.Society.Id && m.Role == "chairperson")!.Name
                        }
                    }).FirstOrDefault()
                }).FirstOrDefaultAsync();


            if (result == null)
            {
                throw new NotFoundException("Requisition not found.");
            }

            string member = result.Event!.Society.Member;

            return new RequisitionDetailsForChairperson()
            {
                Id = result.Id,
                RequestedDate = result.RequestedDate,
                RequestedAmount = result.RequestAmount,
                Status = StatusMap.GetValueOrDefault(result.Status, "Unknown"),
                ReviewMessage = result.ReviewMessage,
                Subject = result.Subject,
                Body = result.Body,
                ChairpersonName = member,
                //Event = er.Event.
                Event = new Event
                {
                    Id = result!.Event!.Id,
                    Name = result.Event.Name,
                    EventDate = result.Event.EventDate,
                    StartTime = result.Event.StartTime,
                    EndTime = result.Event.EndTime,
                    Status = null!,
                    Requirements = result.Event.Requirements,
                    Society = new Society
                    {
                        Id = result.Event.Society.Id,
                        Name = result.Event.Society.Name,
                        Members = new List<Member>
                        {
                            new Member { Name = result.Event.Society.Member, Username = null!, Role = null! }
                        }
                    }
                }
            };
        }

        public async Task UpdateEventRequisition(Guid requisitionId, UpdateEventRequisitionDto updateEventRequisition)
        {
            var transaction = await _dB.Database.BeginTransactionAsync();

            // Step 1: get non-financial requirement names from new event
            var newNonFinancialReqNames = updateEventRequisition.EventRequirements
                .Where(r => r.Type == "non-financial")
                .Select(r => r.Name)
                .ToList();

            // Step 2: check overlapping events + matching requirements
            var conflictingEvent = await _dB.Events
                .Include(e => e.Requirements)
                .FirstOrDefaultAsync(e =>
                    e.Status == "pending" &&
                    e.EventDate.Date == updateEventRequisition.EventDate.Date &&

                    // exclude self (important if updating)
                    e.RequisitionId != requisitionId &&

                    // TIME OVERLAP CHECK
                    updateEventRequisition.StartTime < e.EndTime &&
                    updateEventRequisition.EndTime > e.StartTime &&

                    // REQUIREMENT CONFLICT CHECK
                    e.Requirements.Any(r =>
                        r.Type == "non-financial" &&
                        newNonFinancialReqNames.Contains(r.Name)
                    )
                );

            // Step 3: throw exception if conflict found
            if (conflictingEvent != null)
            {
                var conflictingNames = conflictingEvent.Requirements
                    .Where(r =>
                        r.Type == "non-financial" &&
                        newNonFinancialReqNames.Contains(r.Name))
                    .Select(r => r.Name)
                    .Distinct();

                throw new BusinessRuleException(
                    $"Non-financial requirements '{string.Join(", ", conflictingNames)}' " +
                    $"already exist in event '{conflictingEvent.Name}'"
                );
            }

            try
            {

                var requisition = await _dB.EventRequisitions.FindAsync(requisitionId);

                if (requisition == null)
                    throw new NotFoundException("Requisition Not Found");

                requisition.Subject = updateEventRequisition.Subject;
                requisition.Body = updateEventRequisition.Body;
                requisition.RequestedDate = updateEventRequisition.RequestedDate;
                requisition.RequestAmount = updateEventRequisition.RequestedAmount;

                requisition.Status = requisition.Status switch
                {
                    "B" => "A",
                    "D" => "C",
                    "F" => "E",
                    _ => requisition.Status
                };

                // Get Event
                var _event = await _dB.Events
                        .Include(e => e.Requirements)
                        .FirstOrDefaultAsync(e => e.RequisitionId == requisitionId);

                if (_event == null) throw new NotFoundException("Event not found");

                // Remove requirements
                if (_event != null)
                {
                    _dB.EventRequirements.RemoveRange(_event.Requirements);
                }
                else
                {
                    throw new NotFoundException("Event not found");
                }

                _event.EventDate = updateEventRequisition.EventDate;
                _event.StartTime = updateEventRequisition.StartTime;
                _event.EndTime = updateEventRequisition.EndTime;

                _dB.Events.Update(_event);

                foreach (var req in updateEventRequisition.EventRequirements)
                {
                    EventRequirement eventRequirement = new EventRequirement()
                    {
                        Name = req.Name,
                        Type = req.Type,
                        Quantity = req.Quantity,
                        Price = req.Price,
                        EventId = _event.Id
                    };
                    await _dB.EventRequirements.AddAsync(eventRequirement);
                }
                
                _dB.EventRequisitions.Update(requisition);
                await _dB.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException("Unable to update requisitoin. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // goes to 500 handler
            }
        }

        public async Task<List<RequisitionDetailsForChairperson>> GetRequisitionDetailsForChairperson(Guid memberId)
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
                     Event = er.Events!.Select(e => new
                     {
                         e.Id,
                         e.Name,
                         e.EventDate,
                         e.StartTime,
                         e.EndTime,
                         Requirements = e.Requirements.ToList(),
                         Society = new
                         {
                             e.Society!.Id,
                             e.Society.Name,
                             Member = e.Society.Members!.FirstOrDefault(m => m.Id == memberId)!.Name
                         }
                     }).FirstOrDefault()
                 })
                .AsNoTracking().ToListAsync();


            if (!result.Any())
            {
                throw new NotFoundException("Requisition not found.");
            }

            string member = result.FirstOrDefault()!.Event!.Society.Member;

            return result.Select(er => new RequisitionDetailsForChairperson()
            {
                Id = er.Id,
                RequestedDate = er.RequestedDate,
                RequestedAmount = er.RequestAmount,
                Status = StatusMap.GetValueOrDefault(er.Status, "Unknown"),
                ReviewMessage = er.ReviewMessage,
                Subject = er.Subject,
                Body = er.Body,
                ChairpersonName = member,
                //Event = er.Event.
                Event = new Event
                {
                    Id = er!.Event!.Id,
                    Name = er.Event.Name,
                    EventDate = er.Event.EventDate,
                    StartTime = er.Event.StartTime,
                    EndTime = er.Event.EndTime,
                    Status = null!,
                    Requirements = er.Event.Requirements,
                    Society = new Society
                    {
                        Id = er.Event.Society.Id,
                        Name = er.Event.Society.Name,
                        Members = new List<Member>
                        {
                            new Member { Name = er.Event.Society.Member, Username = null!, Role = null! }
                        }
                    }
                }
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

        public async Task<List<RequisitionDetailsForAdministration>> GetRequisitionDetailsForAdministration(char status)
        {
            // This is comment
            var result = await _dB.EventRequisitions
               .Where(er => er.Status == status.ToString())
               .Select(er => new
               {
                   er.Id,
                   er.RequestedDate,
                   er.RequestAmount,
                   er.Status,
                   er.ReviewMessage,
                   er.Subject,
                   er.Body,
                   Event = er.Events!.Select(e => new
                   {
                       e.Id,
                       e.Name,
                       e.EventDate,
                       e.StartTime,
                       e.EndTime,
                       Requirements = e.Requirements.ToList(),
                       Society = new
                       {
                           e.Society!.Id,
                           e.Society.Name,
                           Member = e.Society.Members!.FirstOrDefault(m => m.SocietyId == e.Id && m.Role == "Chairperson")!.Name
                       }
                   }).FirstOrDefault()
               })
               .AsNoTracking().ToListAsync();

            if (!result.Any())
            {
                throw new NotFoundException("Requisition not found.");
            }

            string member = result.FirstOrDefault()!.Event!.Society.Member;



            //return result
            //    .Where(er => er.Event != null) // Added by jason
            //    .Select(er => new RequisitionDetailsForChairperson()

            return result
                //.Where(er => er.Event != null) // Added by jason
                .Select(er => new RequisitionDetailsForAdministration()

            {
                Id = er.Id,
                RequestedDate = er.RequestedDate,
                RequestedAmount = er.RequestAmount,
                Status = StatusMap.GetValueOrDefault(er.Status, "Unknown"),
                ReviewMessage = er.ReviewMessage,
                Subject = er.Subject,
                Body = er.Body,
                ChairpersonName = member,
                //Event = er.Event.
                Event = new Event
                {
                    Id = er!.Event!.Id,
                    Name = er.Event.Name,
                    EventDate = er.Event.EventDate,
                    StartTime = er.Event.StartTime,
                    EndTime = er.Event.EndTime,
                    Status = null!,
                    Requirements = er.Event.Requirements,
                    Society = new Society
                    {
                        Id = er.Event.Society.Id,
                        Name = er.Event.Society.Name,
                        Members = new List<Member>
                        {
                            new Member { Name = er.Event.Society.Member, Username = null!, Role = null! }
                        }
                    }
                }
            }).ToList();
        }

        //    public async Task<EventRequisitionDetailsDto> GetEventRequisitionDetails(Guid requisitionId)
        //    {
        //        // var eventsIds = await _dB.Events
        //        //     .Where(e => _dB.Members
        //        //     .Any(m => m.Id == memberId && e.SocietyId == m.SocietyId))
        //        //     .Select(e => e.Id)
        //        //     .ToListAsync();

        //        // List<EventRequisition> eventRequisitions = new();

        //        // foreach (var eventId in eventsIds)
        //        // {
        //        //     var requisition = await _dB.EventRequisitions
        //        //         .Where(er => er.Status == "pending")
        //        //         .FirstOrDefaultAsync(er => er.EventId == eventId);
        //        //     if (requisition is not null)
        //        //     {
        //        //         eventRequisitions.Add(requisition);
        //        //     }
        //        // }

        //        // List<PendingEventRequisitionsDto> pendingEventRequisitionsList = new List<PendingEventRequisitionsDto>();

        //        // foreach (var requisition in eventRequisitions)
        //        // {
        //        //     var requirements = await _dB.EventRequirements
        //        //     .Where(e => requisition.EventId == e.EventId)
        //        //     .ToListAsync();

        //        //     PendingEventRequisitionsDto pendingEventRequisitions = new PendingEventRequisitionsDto()
        //        //     {
        //        //         Id = requisition.Id,
        //        //         Body = requisition.Body,
        //        //         Subject = requisition.Subject,
        //        //         Status = requisition.Status,
        //        //         EventRequirement = _dB.EventRequirements
        //        //                 .Where(req => req.EventId == requisition.EventId)
        //        //                 .Select(req => new EventRequirementDto
        //        //                 {
        //        //                     Type = req.Type,
        //        //                     Name = req.Name,
        //        //                     Quantity = req.Quantity,
        //        //                     Price = req.Price
        //        //                 }).ToList()
        //        //     };

        //        //     pendingEventRequisitionsList.Add(pendingEventRequisitions);
        //        // }
        //        // return pendingEventRequisitionsList;


        //        //var result = await _dB.EventRequisitions
        //        //.Where(er => er.Status == "pending" &&
        //        //            _dB.Events.Any(e => e.Id == er.EventId && 
        //        //                _dB.Members.Any(m => m.Id == memberId && 
        //        //                                    m.SocietyId == e.SocietyId))) 
        //        //.Select(er => new EventRequisitionDetailsDto
        //        //{
        //        //    Id = er.Id,
        //        //    Body = er.Body,
        //        //    Subject = er.Subject,
        //        //    Status = er.Status,
        //        //    EventRequirements = _dB.EventRequirements
        //        //        .Where(req => req.EventId == er.EventId)
        //        //        .Select(req => new EventRequirementDto
        //        //        {
        //        //            Type = req.Type,
        //        //            Name = req.Name,
        //        //            Quantity = req.Quantity,
        //        //            Price = req.Price
        //        //        }).ToList()
        //        //})
        //        //.ToListAsync();

        //        var result = await _dB.EventRequisitions
        //            .Where(er => er.Id == requisitionId)
        //            .Select(er => new EventRequisitionDetailsDto()
        //            {
        //                Id = er.Id,
        //                Subject = er.Subject,
        //                EventDate = _dB.EventRequisitions
        //                        .Where(er => er.Id == requisitionId)
        //                        .Select(er => er._event!.Date)
        //                        .FirstOrDefault(),
        //                RequestedDate = er.RequestedDate,
        //                Body = er.Body,
        //                EventRequirements = _dB.EventRequirements
        //                        .Where(req => req.EventId == er.EventId)
        //                        .Select(req => new EventRequirementDto
        //                        {
        //                            Type = req.Type,
        //                            Name = req.Name,
        //                            Quantity = req.Quantity,
        //                            Price = req.Price
        //                        }).ToList(),
        //                SocietyName = _dB.EventRequisitions
        //                        .Where(er => er.Id == requisitionId)
        //                        .Select(er => er._event!.Society!.Name)  // If navigation properties are set up
        //                        .FirstOrDefault()!,
        //                ChairpersonName = _dB.Members
        //                        .Where(m => m.Role == "chairperson" &&
        //                                   m.SocietyId == _dB.Events
        //                                       .Where(e => e.Id == er.EventId)
        //                                       .Select(e => e.SocietyId)
        //                                       .FirstOrDefault())
        //                        .Select(m => m.Name)
        //                        .FirstOrDefault()!

        //            })
        //            .FirstOrDefaultAsync();

        //        if (result == null)
        //            throw new NotFoundException("Requisition Not Found");

        //        return result;
        //    }

        public async Task DeleteEventRequisition(Guid requisitionId)
        {
            var transaction = await _dB.Database.BeginTransactionAsync();

            try
            {
                var requisition = await _dB.EventRequisitions.FindAsync(requisitionId);

                if (requisition == null)
                    throw new NotFoundException("Requisition Not Found");

                // Remove requisition id from old events
                var oldEventList = await _dB.Events.Where(e => e.RequisitionId == requisitionId).ToListAsync();

                foreach (var _event in oldEventList)
                {
                    _event.RequisitionId = null;
                    _event.Status = "pending";
                }

                _dB.EventRequisitions.Remove(requisition);
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


        //    // Chairperson History
        //    public async Task<List<EventRequisitionHistoryDto>> GetEventRequisitionHistory(Guid memberId)
        //    {
        //        var societyId = await _dB.Members
        //                .Where(m => memberId == m.Id)
        //                .Select(m => m.SocietyId)
        //                .FirstOrDefaultAsync();

        //        if (societyId == Guid.Empty)
        //            throw new NotFoundException("Society not found");

        //        var acceptedRequisition = await _dB.EventRequisitions
        //                .Where(er => er.Status == "G" || er.Status == "H" || er.Status == "I" || er.Status == "J"
        //                && er._event!.SocietyId == societyId)
        //                .Select(er => new EventRequisitionHistoryDto()
        //                {
        //                    RequisitionId = er.Id,
        //                    EventId = er.EventId,
        //                    EventName = er._event!.Name,
        //                    RequisitionStatus = er.Status,
        //                    RequestedDate = er.RequestedDate,
        //                    AllocatedDate = er.AllocatedDate,
        //                    RequestedAmount = er.RequestAmount,
        //                    AllocatedAmount = er.RequestAmount,
        //                    BiitContribution = er.BiitContribution
        //                })
        //                .AsNoTracking()
        //                .ToListAsync();

        //        var dto = acceptedRequisition
        //                .Select(er => new EventRequisitionHistoryDto()
        //                {
        //                    RequisitionId = er.RequisitionId,
        //                    EventId = er.EventId,
        //                    EventName = er.EventName,
        //                    RequisitionStatus = StatusMap.GetValueOrDefault(er.RequisitionStatus, "Unknown"),
        //                    RequestedDate = er.RequestedDate,
        //                    AllocatedDate = er.AllocatedDate,
        //                    RequestedAmount = er.RequestedAmount,
        //                    AllocatedAmount = er.AllocatedAmount,
        //                    BiitContribution = er.BiitContribution
        //                })
        //                .ToList();

        //        return dto;
        //    }

        //    // For student affairs and administration to view pending requisitions list
        //    public async Task<List<ViewRequisitionRequestDetailsDto>> GetPendingEventRequisitions(char status)
        //    {
        //        var result = await _dB.EventRequisitions
        //            .Where(er => er.Status == status.ToString())
        //            .Select(er => new ViewRequisitionRequestDetailsDto()
        //            {
        //                Id = er.Id,
        //                EventName = er._event!.Name,
        //                EventDate = er._event!.Date,
        //                RequestedDate = er.RequestedDate,
        //                SocietyName = er._event.Society!.Name,
        //                RequestedAmount = er.RequestAmount,
        //                AllotedAmount = er.AllocatedAmount,
        //                BiitContribution = er.BiitContribution
        //            })
        //            .AsNoTracking()
        //            .ToListAsync();

        //        return result;
        //    }

        //    // Reject by Admin
        //    // Accept by Admin
        //    // Reject by SA
        //    // Budget released by finance
        public async Task RejectEventRequisition(Guid requisitionId, ReviewEventRequisitionDto reviewEventRequisitionDto)
        {
            var transaction = await _dB.Database.BeginTransactionAsync();
            try
            {
                var requisition = await _dB.EventRequisitions.FindAsync(requisitionId);

                if (requisition == null)
                    throw new NotFoundException("Requisition Not Found");

                // Minus credits from yearly if budget release by finance
                // if (reviewEventRequisitionDto.Status == "G")
                // {
                //     // Get the latest yearly budget for the society
                //     var yearlyBudget = await _dB.YearlyBudgets
                //         .Where(yb => yb.SocietyId == requisition._event!.SocietyId)
                //         .OrderByDescending(yb => yb.RequestedDate)
                //         .FirstOrDefaultAsync();

                //     if (yearlyBudget == null)
                //         throw new NotFoundException("Yearly Budget Not Found");

                //     yearlyBudget.Credits -= requisition.RequestAmount;
                //     _dB.YearlyBudgets.Update(yearlyBudget);
                // }

                requisition.Status = reviewEventRequisitionDto.Status;
                requisition.ReviewMessage = reviewEventRequisitionDto.ReviewMessage;

                _dB.EventRequisitions.Update(requisition);
                await _dB.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException("Unable to review event requisition. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // goes to 500 handler
            }
        }

        public async Task ApproveEventRequisitionBySA(Guid requisitionId, ApproveEventRequisitionDto approveEventRequisitionDto)
        {
            var requisition = await _dB.EventRequisitions.FindAsync(requisitionId);

            if (requisition == null)
                throw new NotFoundException("Requisition Not Found");

            requisition.Status = "C";
            requisition.AllocatedDate = approveEventRequisitionDto.AllocatedDate;
            requisition.AllocatedAmount = approveEventRequisitionDto.AllocatedAmount;
            requisition.BiitContribution = approveEventRequisitionDto.BiitContribution;

            _dB.EventRequisitions.Update(requisition);
            await _dB.SaveChangesAsync();
        }

        //public async Task<List<ViewRequisitionDetailsForFinanceDto>> ViewRequisitionDetailsForFinance()
        //{
        //    var acceptedRequisition = await _dB.EventRequisitions
        //            .Where(er => er.Status == "E")
        //            .Select(er => new ViewRequisitionDetailsForFinanceDto()
        //            {
        //                RequisitionId = er.Id,
        //                ChairpersonName = er._event!.Society!.Members
        //                        .Where(m => m.Role == "chairperson")
        //                        .Select(m => m.Name)
        //                        .FirstOrDefault() ?? "N/A",
        //                SocietyName = er._event!.Society!.Name,
        //                EventName = er._event!.Name,
        //                EventDate = er._event.Date,
        //                AllotedBudget = er.AllocatedAmount,
        //                BiitContribution = er.BiitContribution
        //            })
        //            .AsNoTracking()
        //            .ToListAsync();

        //    return acceptedRequisition;
        //}

        //    public async Task<List<ViewRequisitionDetailsForFinanceHistoryDto>> ViewRequisitionDetailsForFinanceHistory()
        //    {
        //        var acceptedRequisition = await _dB.EventRequisitions
        //                .Where(er => er.Status == "G" || er.Status == "H" || er.Status == "I" || er.Status == "J")
        //                .Select(er => new
        //                {
        //                    EventId = er.EventId,
        //                    RequisitionId = er.Id,
        //                    ChairpersonName = er._event!.Society!.Members
        //                            .Where(m => m.Role == "chairperson")
        //                            .Select(m => m.Name)
        //                            .FirstOrDefault() ?? "N/A",
        //                    SocietyName = er._event!.Society!.Name,
        //                    EventName = er._event!.Name,
        //                    EventDate = er._event.Date,
        //                    AllotedBudget = er.AllocatedAmount,
        //                    BiitContribution = er.BiitContribution,
        //                    Status = er.Status
        //                })
        //                .AsNoTracking()
        //                .ToListAsync();

        //        var dto = acceptedRequisition
        //                .Select(dto => new ViewRequisitionDetailsForFinanceHistoryDto()
        //                {
        //                    EventId = dto.EventId,
        //                    RequisitionId = dto.RequisitionId,
        //                    ChairpersonName = dto.ChairpersonName,
        //                    SocietyName = dto.SocietyName,
        //                    EventName = dto.EventName,
        //                    EventDate = dto.EventDate,
        //                    AllotedBudget = dto.AllotedBudget,
        //                    BiitContribution = dto.BiitContribution,
        //                    Status = StatusMap.GetValueOrDefault(dto.Status, "Unknown")
        //                })
        //                .ToList();

        //        return dto;
        //    }

        //    // Requisition list for Student Affairs
        //    public async Task<List<ViewRequisitionDetailsForStudentAffairsDto>> ViewRequisitionDetailsForStudentAffairs()
        //    {
        //        var result = await _dB.EventRequisitions
        //                .Where(er => er.Status != "A")
        //                .Select(er => new
        //                {
        //                    RequisitionId = er.Id,
        //                    ChairpersonName = er._event!.Society!.Members
        //                            .Where(m => m.Role == "chairperson")
        //                            .Select(m => m.Name)
        //                            .FirstOrDefault() ?? "N/A",
        //                    SocietyName = er._event!.Society!.Name,
        //                    EventName = er._event!.Name,
        //                    EventDate = er._event.Date,
        //                    Status = er.Status,
        //                    ReviewMessage = er.ReviewMessage,
        //                    AllotedBudget = er.AllocatedAmount,// Add this property to match usage below
        //                    BiitContribution = er.BiitContribution // Add this property to match usage below
        //                })
        //                .AsNoTracking()
        //                .ToListAsync();


        //        var acceptedRequisition = result
        //            .Select(er => new ViewRequisitionDetailsForStudentAffairsDto()
        //            {
        //                RequisitionId = er.RequisitionId,
        //                ChairpersonName = er.ChairpersonName,
        //                SocietyName = er.SocietyName,
        //                EventName = er.EventName,
        //                EventDate = er.EventDate,
        //                AllotedBudget = er.AllotedBudget,
        //                BiitContribution = er.BiitContribution,
        //                Status = StatusMap.GetValueOrDefault(er.Status, "Unknown"),
        //                ReviewMessage = er.ReviewMessage
        //            })
        //                .ToList();

        //        return acceptedRequisition;
        //    }

        //    public async Task UpdateRequisitionStatus(Guid requisitionId)
        //    {
        //        var requisition = await _dB.EventRequisitions.Where(er => er.Id == requisitionId).FirstOrDefaultAsync();

        //        if (requisition == null)
        //            throw new NotFoundException("Requisition Not Found");

        //        requisition.Status = "J";

        //        _dB.EventRequisitions.Update(requisition);
        //        await _dB.SaveChangesAsync();
        //    }
    }
}
