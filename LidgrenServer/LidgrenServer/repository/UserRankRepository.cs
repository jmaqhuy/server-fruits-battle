using LidgrenServer.Data;
using LidgrenServer.Models;
using Microsoft.EntityFrameworkCore;

namespace LidgrenServer.repository
{
    public class UserRankRepository
    {
        private ApplicationDataContext _context;
        public UserRankRepository(ApplicationDataContext context)
        {
            _context = context;
        }
        public async Task<UserRankModel?> GetUserRank(int userId, int seasonId)
        {
            return await _context.UserRanks
                .Include(ur => ur.Rank)
                .Include(ur => ur.Season)
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.SeasonId == seasonId);
        }
        public async Task CreateNewUserRank(UserRankModel userRankModel)
        {
            await _context.UserRanks.AddAsync(userRankModel);
            await _context.SaveChangesAsync();
        }
    }
}
