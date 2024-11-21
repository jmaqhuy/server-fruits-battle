using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LidgrenServer.Models;
using LidgrenServer.Services;

namespace LidgrenServer.Controllers
{
    public class LoginHistoryController
    {
        private readonly LoginHistoryService _historyService;

        public LoginHistoryController(LoginHistoryService historyService)
        {
            _historyService = historyService;
        }

        public async Task NewUserLoginAsync(int userId, string deviceId)
        {
            var loginHistory = new LoginHistory
            {
                UserId = userId,
                DeviceId = deviceId,
                LoginTime = DateTime.Now
            };

            await _historyService.NewUserLoginAsync(loginHistory);
        }
    }
}
