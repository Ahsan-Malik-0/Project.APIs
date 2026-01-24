using Project.APIs.Database;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Services
{
    public class EventRequisitionService(DB _dB)
    {
        public async Task CreateEventRequisition(EventRequisitionDto newRequisition)
        {
            EventRequisition eventRequisition = new EventRequisition()
            {
                Subject = newRequisition.Subject,
                Body = newRequisition.Body,
                SocietyId = newRequisition.SocietyId,
                Status = "pending",
            };
                
            await _dB.EventRequisitions.AddAsync(eventRequisition);
            await _dB.SaveChangesAsync();
        }


    }
}
