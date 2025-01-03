using LidgrenServer.Data;
using LidgrenServer.Models;
using Microsoft.EntityFrameworkCore;

namespace LidgrenServer.repository
{
    public class SeasonRepository
    {
        public ApplicationDataContext _context;
        public SeasonRepository(ApplicationDataContext context) { _context = context; }
        public async Task CreateNewSeasonAsync(SeasonModel season)
        {
            await _context.AddAsync(season);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SeasonModel>> GetAllSeasonAsync()
        {
            return await _context.Seasons.ToListAsync();
        }

        public async Task<SeasonModel?> GetCurrentSeasonAsync()
        {
            var currentDate = DateTime.Now;
            return await _context.Seasons
                .FirstOrDefaultAsync(s => s.StartDate <= currentDate && s.EndDate >= currentDate);
        }
    }
}
