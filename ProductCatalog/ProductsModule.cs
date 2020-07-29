using Nancy;
using System;

namespace ProductCatalog
{
    public class ProductsModule : NancyModule

    {
        public ProductsModule(ProductStore productStore) : base("/products")
        {
            Get("", _ =>
            {
                string productIdsString = this.Request.Query.productIds;
                var productIds = ParseProductIdsFromQueryString(productIdsString);
                var products = productStore.GetProductsByIds(productIds);
                return
                Negotiate
                .WithModel(products)
                .WithHeader("cache-control", "max-age:86400");
            });
        }

        private object ParseProductIdsFromQueryString(string productIdsString)
        {
            throw new NotImplementedException();
        }
    } 
}
