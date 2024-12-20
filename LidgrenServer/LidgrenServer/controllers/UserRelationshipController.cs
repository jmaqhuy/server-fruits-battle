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


        public async Task<List<UserModel>> GetAllFriendsListAsync(int userId)
        {
            return await _relationshipService.GetAllFriends(userId);
        }
        public async Task<List<UserModel>> GetFriendsRequestListAsync(int userId)
        {
            return await _relationshipService.GetFriendRequest(userId);
        }
        public async Task<List<UserModel>> GetSentFriendsListAsync(int userId)
        {
            return await _relationshipService.GetSentFriend(userId);
        }
        public async Task<List<UserModel>> GetSuggestFriendListAsync(int userId)
        {
            return await _relationshipService.GetUnrelatedUsers(userId);
        }
        public async Task<List<UserModel>> GetSearchedPlayerListAsync(int userId)
        {
            return await _relationshipService.GetSearchedUsers(userId);
        }
        public async Task<List<UserModel>> GetBlockFriendsListAsync(int userId)
        {
            return await _relationshipService.GetBlockedFriends(userId);
        }
        public async Task<bool> AddFriend(int userIdA, int userIdB)
        {
            return await _relationshipService.AddFriend(userIdA, userIdB);
        }

    }
}
