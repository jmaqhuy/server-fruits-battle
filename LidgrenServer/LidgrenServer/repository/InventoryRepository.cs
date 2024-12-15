using LidgrenServer.Data;
using LidgrenServer.Models;
using Microsoft.EntityFrameworkCore;

namespace LidgrenServer.repository
{
    public class InventoryRepository
    {
        private ApplicationDataContext _context;

        public InventoryRepository(ApplicationDataContext context)
        {
            _context = context;
        }
    }
}
