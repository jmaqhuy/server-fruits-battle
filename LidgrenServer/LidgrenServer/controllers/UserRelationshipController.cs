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
        public async Task<List<UserModel>> GetSearchedPlayerAsync(string username1, string username2)
        {
            return await _relationshipService.GetSearchedUsers(username1, username2);
        }
        public async Task<List<UserModel>> GetBlockFriendsListAsync(int userId)
        {
            return await _relationshipService.GetBlockedFriends(userId);
        }
        public async Task<bool> AddFriend(int userIdA, int userIdB)
        {
            return await _relationshipService.AddFriend(userIdA, userIdB);
        }
        public async Task<bool> DeleteFriend(int userIdA, int userIdB)
        {
            return await _relationshipService.DeleteFriend(userIdA, userIdB);
        }
        public async Task<bool> AcceptFriendInvite(int userIdA, int userIdB)
        {
            return await _relationshipService.AcceptFriendInvite(userIdA, userIdB);
        }
        public async Task<bool> CancelFriendRequest(int userIdA, int userIdB)
        {
            return await _relationshipService.CancelFriendRequest(userIdA, userIdB);
        }
        public async Task<bool> BlockFriend(int userIdA, int userIdB)
        {
            return await _relationshipService.BlockFriend(userIdA, userIdB);
        }
        public async Task<bool> UnBlockFriend(int userIdA, int userIdB)
        {
            return await _relationshipService.UnBlockFriend(userIdA, userIdB);
        }
    }
}
