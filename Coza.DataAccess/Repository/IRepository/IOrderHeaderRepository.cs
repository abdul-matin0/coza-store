using Coza.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coza.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
    }
}
