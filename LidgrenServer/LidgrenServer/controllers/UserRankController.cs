using LidgrenServer.Controllers;
using LidgrenServer.Models;
using LidgrenServer.services;

namespace LidgrenServer.controllers
{
    public class UserRankController
    {
        private UserRankService _service;
        private SeasonController _seasonController;
        private UserController _userController;
        public UserRankController(UserRankService service, SeasonController sc, UserController uc)
        {
            _service = service;
            _seasonController = sc;
            _userController = uc;
        }

        public async Task<UserRankModel> GetUserRank(string username, int seasonId = 0)
        {
            var uId = await _userController.GetUserIdByUsernameAsync(username);
            if (seasonId == 0)
            {
                seasonId = (await _seasonController.GetCurrentSeasonAync()).Id;
            }
            return await _service.GetUserRank(uId, seasonId);
        }
    }
}
