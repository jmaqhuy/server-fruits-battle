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

        public async Task NewUserLoginAsync(int userId)
        {
            var loginHistory = new LoginHistoryModel
            {
                UserId = userId,
                LoginTime = DateTime.Now,
                IsLoginNow = true
            };

            await _historyService.NewUserLoginAsync(loginHistory);
        }

        public async Task<bool> UserLogoutAsync(int userId)
        {
            var lh = _historyService.GetCurrentLoginAsync(userId).Result;
            if (lh != null)
            {
                lh.LogoutTime = DateTime.Now;
                lh.IsLoginNow = false;
                await _historyService.UserLogoutAsync(lh);
                return true;
            }
            return false;
        }

        public async Task<bool> UserOnlineNow(int user)
        {
            var lh = await _historyService.GetCurrentLoginAsync(user);
            if (lh != null) { return true; }
            return false;
        }

    }
}
