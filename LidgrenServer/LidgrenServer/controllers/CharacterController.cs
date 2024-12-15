using LidgrenServer.services;

namespace LidgrenServer.controllers
{
    public class CharacterController
    {
        private readonly CharacterService _characterService;
        public CharacterController(CharacterService characterService)
        {
            _characterService = characterService;
        }

        public async Task CreateSampleCharacterAsync()
        {
            await _characterService.CreateCharacterAsync(
                new Models.CharacterModel(){
                    Name = "Banana",
                    Hp = 1000,
                    Damage = 100,
                    Armor = 100,
                    Luck = 5,
                });
        }
        public async Task<int> CharacterCountAsync()
        {
            return await _characterService.CharacterCountAsync();
        }
    }
}
