using LidgrenServer.Models;
using LidgrenServer.Packets;
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

        public async Task UpdateUserCharacter(CharacterPacket cp)
        {
            var ucm = await _repository.GetUserCharacterModel(cp.UserCharacterId);
            if (ucm != null)
            {
                ucm.HpPoint = cp.HpPoint;
                ucm.DamagePoint = cp.DamagePoint;
                ucm.LuckPoint = cp.LuckPoint;
                ucm.ArmorPoint = cp.ArmorPoint;
                await _repository.UpdateUserCharacter(ucm);
            }
        }
    }
}
