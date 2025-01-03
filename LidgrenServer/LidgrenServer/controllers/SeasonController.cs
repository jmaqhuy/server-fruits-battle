using LidgrenServer.Models;
using LidgrenServer.services;

namespace LidgrenServer.controllers
{
    public class SeasonController
    {
        private SeasonService _service;
        public SeasonController(SeasonService service)
        {
            _service = service;
        }
        public async Task<List<SeasonModel>> GetAllSeasonAsync()
        {
            return await _service.GetAllSeasonAsync();
        }
        public async Task<SeasonModel?> GetCurrentSeasonAync()
        {
            return await _service.GetCurrentSeasonAync();
        }
        public async Task CreateNewSeasonAsync(string name, DateTime start, DateTime end)
        {
            await _service.CreateNewSeasonAsync(name, start, end);
        }
    }
}
