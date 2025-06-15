using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository CatRepo { get; private set; }
        public IProductRepository PdtRepo { get; private set; }
        public ICompanyRepository CompRepo { get; private set; }
        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            CatRepo = new CategoryRepository(_db);
            PdtRepo = new ProductRepository(_db);
            CompRepo = new CompanyRepository(_db);
        }
        

        public void Save()
        {
            _db.SaveChanges();
        }
    }
} 
