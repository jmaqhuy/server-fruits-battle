using LidgrenServer.Models;
using LidgrenServer.services;

namespace LidgrenServer.controllers
{
    public class RankController
    {
        private RankService _service;
        public RankController(RankService rankService)
        {
            _service = rankService;
        }
        public async Task<RankModel> GetRankModelAsync(int id)
        {
            return await _service.GetRankModelByIdAsync(id);
        }
        public async Task CreateSampleRanks()
        {
            await _service.CreateDefaultRanks();
        }
        public async Task AddNewRankAsync(string name, string assetName, int maxStar)
        {
            await _service.AddNewRankAsync(
                new RankModel()
                {
                    Name = name,
                    AssetName = assetName,
                    MaxStar = maxStar
                });
        }
    }
}
