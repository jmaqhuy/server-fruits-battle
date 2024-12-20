using LidgrenServer.Data;
using LidgrenServer.Models;
using Microsoft.EntityFrameworkCore;
using static LidgrenServer.Packets.PacketTypes;

namespace LidgrenServer.services
{
    public class UserRelationshipService
    {
        private readonly ApplicationDataContext _dbContext;

        public UserRelationshipService(ApplicationDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Lấy danh sách người dùng không có bất kỳ mối quan hệ nào với userId.
        /// </summary>
        /// <param name="userId">Id của người dùng cần kiểm tra</param>
        /// <returns>Danh sách người dùng không liên quan</returns>


        public async Task<bool> AddFriend(int userIdA, int userIdB)
        {
            RelationshipType type = RelationshipType.PENDING_FIRST_SECOND;
            if (userIdA == userIdB) return false;
            if (userIdA > userIdB)
            {
                //swap 2 id
                userIdA = userIdA ^ userIdB;
                userIdB = userIdA ^ userIdB;
                userIdA = userIdA ^ userIdB;
                type = RelationshipType.PENDING_SECOND_FIRST;
            }
            _dbContext.UserRelationships.Add(new UserRelationship
            {
                UserFirstId = userIdA,
                UserSecondId = userIdB,
                Type = type
            });
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<UserModel>> GetAllFriends(int userId)
        {
            // Get the list of user IDs who are friends with the given userId
            var FriendUserIds = await _dbContext.UserRelationships
                .Where(ur =>
                    (ur.UserFirstId == userId || ur.UserSecondId == userId) &&
                    ur.Type == RelationshipType.FRIENDS)
                .Select(ur => ur.UserFirstId == userId ? ur.UserSecondId : ur.UserFirstId)
                .Distinct()
                .ToListAsync();

            // Query the Users table to retrieve user details for these friends
            var Friends = await _dbContext.Users
                .Where(u => FriendUserIds.Contains(u.Id) && !string.IsNullOrEmpty(u.Display_name))
                .ToListAsync();

            return Friends;
        }
        public async Task<List<UserModel>> GetFriendRequest(int userId)
        {
            // Get the list of user IDs who are friends with the given userId
            var FriendUserIds = await _dbContext.UserRelationships
                .Where(ur =>
                    ur.UserSecondId == userId &&
                    ur.Type == RelationshipType.PENDING_SECOND_FIRST)
                .Select(ur => ur.UserFirstId)
                .Distinct()
                .ToListAsync();

            // Query the Users table to retrieve user details for these friends
            var Friends = await _dbContext.Users
                .Where(u => FriendUserIds.Contains(u.Id) && !string.IsNullOrEmpty(u.Display_name))
                .ToListAsync();
            return Friends;
        }
        public async Task<List<UserModel>> GetSentFriend(int userId)
        {
            // Get the list of user IDs who are friends with the given userId
            var FriendUserIds = await _dbContext.UserRelationships
                .Where(ur =>
                    ur.UserFirstId == userId &&
                    ur.Type == RelationshipType.PENDING_FIRST_SECOND)
                .Select(ur => ur.UserSecondId)
                .Distinct()
                .ToListAsync();

            // Query the Users table to retrieve user details for these friends
            var Friends = await _dbContext.Users
                .Where(u => FriendUserIds.Contains(u.Id) && !string.IsNullOrEmpty(u.Display_name))
                .ToListAsync();

            return Friends;
        }
        public async Task<List<UserModel>> GetUnrelatedUsers(int userId)
        {
            // Lấy danh sách Id liên quan đến userId từ bảng user_relationship
            var relatedUserIds = await _dbContext.UserRelationships
                .Where(ur => ur.UserFirstId == userId || ur.UserSecondId == userId)
                .Select(ur => ur.UserFirstId == userId ? ur.UserSecondId : ur.UserFirstId)
                .Distinct()
                .ToListAsync();

            // Lấy tất cả người dùng không nằm trong danh sách liên quan, sắp xếp ngẫu nhiên và giới hạn số lượng
            var unrelatedUsers = await _dbContext.Users
                .Where(u => u.Id != userId && !relatedUserIds.Contains(u.Id) && !string.IsNullOrEmpty(u.Display_name))
                .OrderBy(u => Guid.NewGuid()) // Randomize
                .Take(10) // Limit results
                .ToListAsync();

            return unrelatedUsers;
        }
        public async Task<List<UserModel>> GetSearchedUsers(int userId)
        {
            // Lấy tất cả người dùng không nằm trong danh sách liên quan, sắp xếp ngẫu nhiên và giới hạn số lượng
            var Friend = await _dbContext.Users
                .Where(u => u.Id == userId && !string.IsNullOrEmpty(u.Display_name))
                .ToListAsync();

            return Friend;
        }
        public async Task<List<UserModel>> GetBlockedFriends(int userId)
        {
            // Get the list of user IDs who are friends with the given userId
            var FriendUserIds = await _dbContext.UserRelationships
                .Where(ur =>
                    (ur.UserFirstId == userId  && ur.Type == RelationshipType.BLOCK_FIRST_SECOND) ||
                    (ur.UserSecondId == userId && ur.Type == RelationshipType.BLOCK_SECOND_FIRST))
                .Select(ur => ur.UserFirstId == userId ? ur.UserSecondId : ur.UserFirstId)
                .Distinct()
                .ToListAsync();

            // Query the Users table to retrieve user details for these friends
            var Friends = await _dbContext.Users
                .Where(u => FriendUserIds.Contains(u.Id) && !string.IsNullOrEmpty(u.Display_name))
                .ToListAsync();
            return Friends;
        }
    }
}
