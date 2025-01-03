using LidgrenServer.Models;
using LidgrenServer.repository;

namespace LidgrenServer.services
{
    public class SeasonService
    {
        private SeasonRepository _repository;
        public SeasonService(SeasonRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<SeasonModel>> GetAllSeasonAsync()
        {
            return await _repository.GetAllSeasonAsync();
        }
        public async Task<SeasonModel?> GetCurrentSeasonAync()
        {
            return await _repository.GetCurrentSeasonAsync();
        }
        public async Task CreateNewSeasonAsync(string name, DateTime startDate, DateTime endDate)
        {
            await _repository.CreateNewSeasonAsync(

                new SeasonModel()
                {
                    Name = name,
                    StartDate = startDate,
                    EndDate = endDate
                });
        }
    }
}
