using Coza.DataAccess.Data;
using Coza.DataAccess.Repository.IRepository;
using Coza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coza.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
