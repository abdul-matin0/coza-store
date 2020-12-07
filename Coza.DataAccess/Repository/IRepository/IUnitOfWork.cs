using System;
using System.Collections.Generic;
using System.Text;

namespace Coza.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        public ICategoryRepository Category { get; }

        void Save();
    }
}
