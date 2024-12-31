using LidgrenServer.Controllers;
using LidgrenServer.Models;
using LidgrenServer.Packets;
using LidgrenServer.services;

namespace LidgrenServer.controllers
{
    

    public class UserCharacterController
    {
        private readonly UserCharacterService _service;
        private readonly UserController _userController;

        public UserCharacterController(UserCharacterService service, UserController userController)
        {
            _service = service;
            _userController = userController;
        }

        public async Task<UserCharacterModel> GetCurrentCharacterAsync(string username)
        {
            var userId = await _userController.GetUserIdByUsernameAsync(username);
            return await GetCurrentCharacterAsync(userId);
        }

        public async Task AddCharacterToNewUser(int userId, int characterId)
        {
            await _service.AddCharacterToUser(userId, characterId, true);
        }

        public async Task<UserCharacterModel> GetCurrentCharacterAsync(int userId)
        {
            return await _service.GetCurrentCharacterAsync(userId);
        }

        public async Task UpdateUserCharacter(CharacterPacket cp)
        {
            await _service.UpdateUserCharacter(cp);
        }
    }
}
