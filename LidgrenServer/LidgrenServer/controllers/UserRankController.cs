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
            var rank = _rankController.GetRankModelAsync(userRankModel.RankId).Result;
            Logging.Info($"User {username}, current star {userRankModel.CurrentStar}");
            userRankModel.CurrentStar += star;
            Logging.Info($"User {username}, Sau khi cong sao {userRankModel.CurrentStar}");
            Logging.Info($"User Star = {userRankModel.CurrentStar}, rank {rank.Name}, rank.MaxStar ={rank.MaxStar} ");
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
                    Logging.Info($"User Star = {userRankModel.CurrentStar} > rank.MaxStar ={rank.MaxStar} ");
                    if (rank.MaxStar == 0) 
                    {
                        userRankModel.CurrentStar++;
                        break;
                    }
                    userRankModel.CurrentStar -= rank.MaxStar;
                    userRankModel.RankId++;
                    rank = _rankController.GetRankModelAsync(userRankModel.Id).Result;
                    Logging.Info($"User {username}, rank cuoi {userRankModel.RankId} {userRankModel.CurrentStar}");
                }
            }
            _service.UpdateUserRank(userRankModel);
        }
    }
}
