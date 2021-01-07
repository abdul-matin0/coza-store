using Coza.DataAccess.Data;
using Coza.DataAccess.Repository.IRepository;
using Coza.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coza.DataAccess.Repository
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private readonly ApplicationDbContext _db;
        public PaymentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Payment obj)
        {
            _db.Update(obj);
        }
    }
}
