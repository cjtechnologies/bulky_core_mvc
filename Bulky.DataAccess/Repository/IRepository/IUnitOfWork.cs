using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CatRepo { get; }
        IProductRepository PdtRepo { get; }
        IProductImageRepository PdtImgRepo { get; }
        ICompanyRepository CompRepo { get; }
        IShoppingCartRepository CartRepo { get; }
        IApplicationUserRepository UserRepo { get; }
        IOrderDetailRepository OrdDetailRepo { get; }
        IOrderHeaderRepository OrdHeaderRepo { get; }
        void Save();
    }
}
