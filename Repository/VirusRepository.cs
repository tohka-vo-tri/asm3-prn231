using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class VirusRepository : GenericRepository<Virus>
    {
        public VirusRepository() { }
        public async Task<Virus?> GetByNameAsync(string virusName)
        {
            if (string.IsNullOrWhiteSpace(virusName))
            {
                return null; // Handle null or empty input as needed
            }

            return await _context.Viruses
                .FirstOrDefaultAsync(v => v.VirusName == virusName); // No casing methods
        }

        public async Task<int?> GetMaxIdAsync()
        {
            return await _context.Viruses.MaxAsync(v => (int?)v.VirusId);
        }

    }
}
