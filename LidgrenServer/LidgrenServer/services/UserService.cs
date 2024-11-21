using LidgrenServer.Data;
using LidgrenServer.model;
using Microsoft.EntityFrameworkCore;

namespace LidgrenServer.services
{
    public class UserService
    {
        private readonly ApplicationDataContext _context;

        public UserService (ApplicationDataContext context)
        {
            _context = context;
        }

        //Read
        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Username == username);
        }

        //Create
        public async Task CreateNewUserAsync(UserModel user)
        {
            //await _context.Users.AddAsync(user);
            //await _context.SaveChangesAsync();
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username);

            if (existingUser == null)
            {
                user.Password = user.HashPassword(user.Password);
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Nếu user đã tồn tại, bạn có thể quyết định xử lý theo cách khác
                throw new InvalidOperationException("User already exists.");
            }
        }

        //Update
        public async Task UpdateUserAsysn(UserModel user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

    }
}
