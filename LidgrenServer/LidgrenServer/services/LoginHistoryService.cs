using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task NewUserLoginAsync(LoginHistory loginHistory)
        {
            await _context.LoginHistories.AddAsync(loginHistory);
            await _context.SaveChangesAsync();
        }

        //Update
        public async Task UserLogoutAsync(LoginHistory loginHistory)
        {
            _context.LoginHistories.Update(loginHistory);
            await _context.SaveChangesAsync();
        }

        //Read
        public async Task<LoginHistory?> GetCurrentLoginAsync(int userId)
        {
            return await _context.LoginHistories
                .FirstOrDefaultAsync(lh =>
                    lh.UserId == userId &&
                    lh.IsLoginNow);
        }

    }
}
