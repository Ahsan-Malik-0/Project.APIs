using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Model;

namespace Project.APIs.Services
{
    public class SocietyService(DB _dB)
    {
        //Get All Societies
        public async Task<List<Society>> GetAllSocieties()
        {
            return await _dB.Societies.ToListAsync();
        }

    }
}
