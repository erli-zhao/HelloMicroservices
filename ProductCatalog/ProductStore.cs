using System;
using System.Threading.Tasks;

namespace ProductCatalog
{
    public class ProductStore : IProductStore
    {
        public Task<Product> GetProductsByIds(object productIds)
        {
            throw new NotImplementedException();
        }
    }
}