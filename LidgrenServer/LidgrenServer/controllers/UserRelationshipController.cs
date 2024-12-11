using LidgrenServer.Models;
using LidgrenServer.services;

namespace LidgrenServer.controllers
{
    public class UserRelationshipController
    {
        private readonly UserRelationshipService _relationshipService;
        public UserRelationshipController(UserRelationshipService relationshipService)
        {
            _relationshipService = relationshipService;
        }

        public async Task<List<UserModel>> GetSuggestFriendListAsync(int userId)
        {
            return await _relationshipService.GetUnrelatedUsers(userId);
        }
        public async Task<bool> AddFriend(int userIdA, int userIdB)
        {
            return await _relationshipService.AddFriend(userIdA, userIdB);
        }

    }
}
