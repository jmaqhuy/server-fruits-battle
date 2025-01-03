using LidgrenServer.Models;
using LidgrenServer.repository;

namespace LidgrenServer.services
{
    public class RankService
    {
        private RankRepository _repository;
        public RankService(RankRepository repository)
        {
            _repository = repository;
        }
        public async Task<RankModel> GetRankModelByIdAsync(int id)
        {
            return await _repository.GetRankModelAsync(id);
        }

        public async Task AddNewRankAsync(RankModel rankModel)
        {
            await _repository.CreateNewRankAsync(rankModel);
        }
        public async Task CreateDefaultRanks()
        {
            await _repository.CreateNewRankAsync(
                new RankModel()
                {
                    Name = "Bronze",
                    AssetName = "Bronze",
                    MaxStar = 3,
                });
            await _repository.CreateNewRankAsync(
                new RankModel()
                {
                    Name = "Silver",
                    AssetName = "Silver",
                    MaxStar = 3,
                });
            await _repository.CreateNewRankAsync(
                new RankModel()
                {
                    Name = "Gold",
                    AssetName = "Gold",
                    MaxStar = 4,
                });
            await _repository.CreateNewRankAsync(
                new RankModel()
                {
                    Name = "Majestic",
                    AssetName = "Majestic",
                    MaxStar = 5,
                });
            await _repository.CreateNewRankAsync(
                new RankModel()
                {
                    Name = "Legendary",
                    AssetName = "Legendary",
                    MaxStar = 0,
                });

        }
    }
}
