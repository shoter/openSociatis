using Common.EntityFramework;
using Entities.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Repository
{
    public class ProductRepository : RepositoryBase<Product, SociatisEntities>, IProductRepository
    {
        public ProductRepository(SociatisEntities context) : base(context)
        {
        }
    }
}
