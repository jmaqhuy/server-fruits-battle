using LidgrenServer.Data;
using LidgrenServer.Models;
using Microsoft.EntityFrameworkCore;

namespace LidgrenServer.repository
{
    public class CharacterRepository
    {
        private readonly ApplicationDataContext _context;

        public CharacterRepository(ApplicationDataContext context)
        {
            _context = context;
        }

        // Get All Character
        public async Task<List<CharacterModel>> GetAllCharacterAsync()
        {
            return await _context.Characters.ToListAsync();
        }

        // Create CRUD
        public async Task CreateCharacterAsync(CharacterModel character)
        {
            await _context.Characters.AddAsync(character);
            await UpdateDatabase();
        }

        // Update CRUD
        public async Task UpdateCharacterAsync(CharacterModel character)
        {
            _context.Characters.Update(character);
            await UpdateDatabase();
        }

        private async Task UpdateDatabase()
        {
            await _context.SaveChangesAsync();
        }
    }
}
