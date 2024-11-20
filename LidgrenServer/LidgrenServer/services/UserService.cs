using LidgrenServer.Data;
using LidgrenServer.model;

namespace LidgrenServer.services
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

    }
}
