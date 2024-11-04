using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class PersionVirusRepository : GenericRepository<PersonVirus>
    {
        public PersionVirusRepository() { }
        public async Task<List<PersonVirus>> GetAllByIdAsync(int personId)
        {
            return await _context.PersonViruses
                .Where(pv => pv.PersonId == personId)
                .Include(pv => pv.Virus) // Include Virus details if needed
                .ToListAsync();
        }
    }
}
