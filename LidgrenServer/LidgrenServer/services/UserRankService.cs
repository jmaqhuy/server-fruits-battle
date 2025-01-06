using LidgrenServer.Models;
using LidgrenServer.repository;

namespace LidgrenServer.services
{
    public class UserRankService
    {
        private UserRankRepository _repository;
        public UserRankService(UserRankRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserRankModel> GetUserRank(int uId, int seasonId)
        {
            var ur = await _repository.GetUserRank(uId, seasonId);
            if (ur == null)
            {
                ur = new UserRankModel()
                {
                    UserId = uId,
                    SeasonId = seasonId,
                    RankId = 1,
                    CurrentStar = 0
                };
                await _repository.CreateNewUserRank(ur);
                return await _repository.GetUserRank(uId, seasonId);
            }
            else
            {
                return ur;
            }

        }
        public void UpdateUserRank(UserRankModel userRank)
        {
            _repository.UpdateUserRank(userRank);
        }
    }
}
