using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;
using ShoppingCart.ShoppingCart;

namespace ShoppingCart
{
    public class ProductCatalogClient: IProductcatalogClient
    {
        private static string productCatalogBaseUrl =@"http://private-05cc8-chapter2productcatalogmicroservice.apiary-mock.com";
        private static string getProductPathTemplate ="/products?productIds=[{0}]";


        /// <summary>
        /// uses Polly's fluent API to set up a retry policy with an exponential back-off
        /// 
        /// </summary>
        private static Polly.Retry.AsyncRetryPolicy exponentialRetryPolicy =
           Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
               3,
               attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)), (ex, _) => Console.WriteLine(ex.ToString()));
        //Wraps calls to the Product Catalog microservice in the retry policy
        public async Task<IEnumerable<ShoppingCartItem>> GetShoppingCartItems(int[] productCatalogIds) =>
            await exponentialRetryPolicy.ExecuteAsync(async () => await GetItemFromCatalogeService(productCatalogIds).ConfigureAwait(false));

        private static async Task<HttpResponseMessage> RequestProductFromProductCatalogue(int[] productCataloguIds)
        {
            var productsResource = string.Format(getProductPathTemplate, string.Join(",", productCataloguIds));
            using (var httpClient=new HttpClient())
            {
                httpClient.BaseAddress = new Uri(productCatalogBaseUrl);
                return await httpClient.GetAsync(productsResource).ConfigureAwait(false);
            }
        }
        private static async Task<IEnumerable<ShoppingCartItem>> ConvertToShoppingCartItems(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var products = JsonConvert.DeserializeObject<List<ProductCatalogueProduct>>(
                await response.Content.ReadAsStringAsync().ConfigureAwait(false)
                ) ;
            return products.Select(p => new ShoppingCartItem(
                int.Parse(p.ProductId),
                p.ProductName,
                p.ProductDescription,
                p.Price));
        }

        private async Task<IEnumerable<ShoppingCartItem>> GetItemFromCatalogeService(int[] productCataloguIds)
        {
            var response = await RequestProductFromProductCatalogue(productCataloguIds).ConfigureAwait(false);
            return await ConvertToShoppingCartItems(response).ConfigureAwait(false);
        }


        

        private class ProductCatalogueProduct
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public Money Price { get; set; }
    }

    }

}
