using System;
using System.Collections.Generic;
using System.Text;

namespace Coza.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        public ICategoryRepository Category { get; }
        public IProductRepository Product { get; }
        public IApplicationUserRepository ApplicationUser { get; }
        public IShoppingCartRepository ShoppingCart { get;  }
        public IOrderHeaderRepository OrderHeader { get; }
        public IOrderDetailsRepository OrderDetails { get; }
        public IPaymentRepository Payment { get; }

        void Save();
    }
}
