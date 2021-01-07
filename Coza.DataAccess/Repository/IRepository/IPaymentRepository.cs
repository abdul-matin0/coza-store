using Coza.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coza.DataAccess.Repository.IRepository
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        void Update(Payment obj);
    }
}
