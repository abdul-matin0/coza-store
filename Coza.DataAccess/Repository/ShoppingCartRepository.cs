using Coza.DataAccess.Data;
using Coza.DataAccess.Repository.IRepository;
using Coza.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coza.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ShoppingCart obj)
        {
            _db.Update(obj);
        }
    }
}
