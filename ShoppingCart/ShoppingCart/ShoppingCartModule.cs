using Nancy;
using Nancy.ModelBinding;
using ShoppingCart.EventFeed;

namespace ShoppingCart.ShoppingCart
{
    public class ShoppingCartModule:NancyModule
    {
        public ShoppingCartModule(IShoppingCartStore shoppingCartStore,
                                  IProductcatalogClient productcatalog,
                                  IEventStore eventStore)
            : base("/shoppingcart")
        {
            Get("/{userid:int}", paramters =>
            {
                var userId = (int)paramters.userid;
                return shoppingCartStore.Get(userId);
            });
            Post("/{userid:int}/items", async (parameters,_) => 
            {
                var productCatalogIds = this.Bind<int[]>();
                var userId = (int)parameters.userid;
                ShoppingCart shoppingCart = await shoppingCartStore.Get(userId);
                var shoppingCartItems=await
                productcatalog
                .GetShoppingCartItems(productCatalogIds)
                .ConfigureAwait(false);

                shoppingCart.AddItems(shoppingCartItems, eventStore);

                await shoppingCartStore.Save(shoppingCart);

                return shoppingCart;
            });

            Delete("/{userid:int}/items", async parameters =>
            {
                var productCatalogIds = this.Bind<int[]>();
                var userId = (int)parameters.userid;
                ShoppingCart shoppingCart = await shoppingCartStore.Get(userId);
                shoppingCart.RemoveItems(productCatalogIds, eventStore);
                await shoppingCartStore.Save(shoppingCart);
                return shoppingCart;
            });
        } 

    }
}
