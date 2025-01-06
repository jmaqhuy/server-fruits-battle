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
        private RankController _rankController;
        public UserRankController(UserRankService service, SeasonController sc, UserController uc, RankController rankController)
        {
            _service = service;
            _seasonController = sc;
            _userController = uc;
            _rankController = rankController;
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

        public void ChangeUserRankStar(string username, int star)
        {
            var userRankModel = this.GetUserRank(username).Result;
            var rank = _rankController.GetRankModelAsync(userRankModel.Id).Result;
            userRankModel.CurrentStar += star;
            while (userRankModel.CurrentStar > rank.MaxStar || userRankModel.CurrentStar < 0)
            {
                if (userRankModel.CurrentStar < 0)
                {
                    if (userRankModel.RankId == 1)
                    {
                        userRankModel.CurrentStar++;
                        break;
                    }
                    userRankModel.RankId--;
                    rank = _rankController.GetRankModelAsync(userRankModel.Id).Result;
                    userRankModel.CurrentStar = --rank.MaxStar;
                }
                else
                {
                    if (rank.MaxStar == 0) 
                    {
                        userRankModel.CurrentStar++;
                        break;
                    }
                    userRankModel.CurrentStar -= rank.MaxStar;
                    userRankModel.RankId++;
                    rank = _rankController.GetRankModelAsync(userRankModel.Id).Result;
                }
            }
            _service.UpdateUserRank(userRankModel);
        }
    }
}
