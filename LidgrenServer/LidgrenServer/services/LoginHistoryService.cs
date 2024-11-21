using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LidgrenServer.Data;
using LidgrenServer.Models;

namespace LidgrenServer.Services
{
    public class LoginHistoryService
    {
        private readonly ApplicationDataContext _context;
        public LoginHistoryService(ApplicationDataContext context)
        {
            _context = context;
        }

        public async Task NewUserLoginAsync(LoginHistory loginHistory)
        {
            await _context.loginHistories.AddAsync(loginHistory);
            await _context.SaveChangesAsync();
        }


    }
}
