using LidgrenServer.Data;
using LidgrenServer.Models;
using Microsoft.EntityFrameworkCore;

namespace LidgrenServer.Services
{
    public class UserService
    {
        private readonly ApplicationDataContext _context;

        public UserService (ApplicationDataContext context)
        {
            _context = context;
        }

        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Username == username);
        }

        public async Task<UserModel> GetUserByUsernameEmailAsync(string username, string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Username == username && user.Email == email);
        }

        //Create CRUD
        public async Task CreateNewUserAsync(UserModel user)
        {
            user.Password = user.HashPassword(user.Password);
            await _context.Users.AddAsync(user);
            await UpdateDatabase();
        }

        //Update CRUD
        public async Task UpdateUserAsysn(UserModel user)
        {
            _context.Users.Update(user);
            await UpdateDatabase();
        }

        public async Task UpdateDatabase()
        {
            await _context.SaveChangesAsync();
        }

    }
}
