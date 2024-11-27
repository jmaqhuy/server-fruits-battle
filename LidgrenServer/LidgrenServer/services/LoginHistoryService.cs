using LidgrenServer.Data;
using LidgrenServer.Models;
using Microsoft.EntityFrameworkCore;

namespace LidgrenServer.Services
{
    public class LoginHistoryService
    {
        private readonly ApplicationDataContext _context;
        public LoginHistoryService(ApplicationDataContext context)
        {
            _context = context;
        }

        //Create
        public async Task NewUserLoginAsync(LoginHistoryModel loginHistory)
        {
            await _context.LoginHistories.AddAsync(loginHistory);
            await _context.SaveChangesAsync();
        }

        //Update
        public async Task UserLogoutAsync(LoginHistoryModel loginHistory)
        {
            _context.LoginHistories.Update(loginHistory);
            await _context.SaveChangesAsync();
        }

        //Read
        public async Task<LoginHistoryModel?> GetCurrentLoginAsync(int userId)
        {
            return await _context.LoginHistories
                .FirstOrDefaultAsync(lh =>
                    lh.UserId == userId &&
                    lh.IsLoginNow);
        }

    }
}
