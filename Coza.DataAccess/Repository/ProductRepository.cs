using Coza.DataAccess.Data;
using Coza.DataAccess.Repository.IRepository;
using Coza.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coza.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var objFromDb = _db.Product.FirstOrDefault(c => c.Id == product.Id);
            if(objFromDb != null)
            {
                if(product.ImageUrl != null)
                {
                    objFromDb.ImageUrl = product.ImageUrl;
                }

                objFromDb.Title = product.Title;
                objFromDb.Description = product.Description;
                objFromDb.Price = product.Price;
                objFromDb.CategoryId = product.CategoryId;
               
            }
        }
    }
}
