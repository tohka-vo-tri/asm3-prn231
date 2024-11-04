using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;

namespace Repository
{
    public class ViroCureUserRepository : GenericRepository<ViroCureUser>
    {
        public ViroCureUserRepository() { }
        public ViroCureUser CheckLogin(string email, string password)
        {
            return _context.ViroCureUsers.Where(x => x.Email.Equals(email) && x.Password.Equals(password)).FirstOrDefault();
        }
    }
}
