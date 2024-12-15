using LidgrenServer.Models;
using LidgrenServer.repository;

namespace LidgrenServer.services
{
    public class CharacterService
    {
        private readonly CharacterRepository _characterRepository;
        public CharacterService(CharacterRepository characterRepository)
        {
            _characterRepository = characterRepository;
        }

        public async Task CreateCharacterAsync(CharacterModel character)
        {
            await _characterRepository.CreateCharacterAsync(character);
        }

        public async Task CreateCharactersAsync(CharacterModel[] characters)
        {
            foreach (var character in characters)
            {
                await CreateCharacterAsync(character);
            }
        }

        public async Task<int> CharacterCountAsync()
        {
            return (await _characterRepository.GetAllCharacterAsync()).Count;
        }


    }
}
