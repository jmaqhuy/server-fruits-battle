using LidgrenServer.Models;
using LidgrenServer.repository;

namespace LidgrenServer.services
{
    public class UserCharacterService
    {
        private readonly UserCharacterRepository _repository;

        public UserCharacterService(UserCharacterRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserCharacterModel> GetCurrentCharacterAsync(int userId)
        {
            return await _repository.GetCurrentCharacter(userId);
        }

        public async Task AddCharacterToUser(int userId, int characterId, bool isSelected)
        {
            await _repository.AddCharacterToUser(
                new UserCharacterModel()
                {
                    UserId = userId,
                    CharacterId = characterId,
                    IsSelected = isSelected
                });
        }
    }
}
