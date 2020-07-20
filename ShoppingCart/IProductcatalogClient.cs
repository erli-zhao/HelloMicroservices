using ShoppingCart.ShoppingCart;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCart
{
    public interface IProductcatalogClient
    {
        Task <IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogIds);
    }
}