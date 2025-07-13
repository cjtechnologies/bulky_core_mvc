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
        public IProductImageRepository PdtImgRepo { get; private set; }
        public ICompanyRepository CompRepo { get; private set; }
        public IShoppingCartRepository CartRepo { get; private set; }
        public IApplicationUserRepository UserRepo { get; private set; }
        public IOrderDetailRepository OrdDetailRepo { get; }
        public IOrderHeaderRepository OrdHeaderRepo { get; }
        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            CatRepo = new CategoryRepository(_db);
            PdtRepo = new ProductRepository(_db);
            PdtImgRepo = new ProductImageRepository(_db);
            CompRepo = new CompanyRepository(_db);
            CartRepo = new ShoppingCartRepository(_db);
            UserRepo = new ApplicationUserRepository(_db);
            OrdDetailRepo = new OrderDetailRepository(_db);
            OrdHeaderRepo = new OrderHeaderRepository(_db);
        }
        

        public void Save()
        {
            _db.SaveChanges();
        }
    }
} 
