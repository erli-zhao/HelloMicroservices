using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalog
{
    public interface IProductStore
    {
        Task<Product> GetProductsByIds(object productIds);
    }
}
