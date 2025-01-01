using LidgrenServer.Data;
using LidgrenServer.Models;
using Microsoft.EntityFrameworkCore;
using static LidgrenServer.Packets.PacketTypes;

namespace LidgrenServer.repository
{
    public class UserCharacterRepository
    {
        private readonly ApplicationDataContext _context;
        public UserCharacterRepository(ApplicationDataContext context)
        {
            _context = context;
        }

        public async Task<UserCharacterModel> GetCurrentCharacter(int userId)
        {
            return await _context.UserCharacters
                .Include(uc => uc.Character)
                .Where(uc => uc.UserId == userId && uc.IsSelected)
                .FirstOrDefaultAsync();
        }

        public async Task<UserCharacterModel?> GetUserCharacterModel(int ucId)
        {
            return await _context.UserCharacters
                .FirstOrDefaultAsync(uc => uc.Id == ucId);
        }

        public async Task AddCharacterToUser(UserCharacterModel uc)
        {
            await _context.AddAsync(uc);
        }

        public async Task UpdateUserCharacter(UserCharacterModel uc)
        {
            _context.UserCharacters.Update(uc);
            await _context.SaveChangesAsync();
        }
    }
}
