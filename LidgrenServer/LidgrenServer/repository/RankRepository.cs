using LidgrenServer.Data;
using LidgrenServer.Models;
using Microsoft.EntityFrameworkCore;

namespace LidgrenServer.repository
{
    public class RankRepository
    {
        private ApplicationDataContext _context;
        public RankRepository(ApplicationDataContext context)
        {
            _context = context;
        }
        public async Task<RankModel> GetRankModelAsync(int id)
        {
            return await _context.Ranks.FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task CreateNewRankAsync(RankModel rankModel)
        {
            _context.Ranks.Add(rankModel);
            await _context.SaveChangesAsync();
        }
    }
}
