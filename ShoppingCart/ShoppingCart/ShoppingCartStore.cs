using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace ShoppingCart.ShoppingCart
{
    public class ShoppingCartStore : IShoppingCartStore
    {
        private readonly string connectionString = @"server=192.168.18.171;port=3306;uid=user;pwd=!wszlj2390;database=microservice;";
        private const string readItesmSql = @"SELECT * FROM  shoppingcart,shoppingCartItems where shoppingCartItems.ShoppingCartId
                                               =shoppingcart.ID and ShoppingCart.UserId=@UserId";
        public async Task<ShoppingCart> Get(int userId)
        {
            using (var conn=new MySqlConnection(connectionString) )
            {
                var item = await conn.QueryAsync<ShoppingCartItem>(readItesmSql, new { UserId = userId });
                return new ShoppingCart(userId, item);
            }
        }


        private const string deleteAllForShoppingCartSql =@"delete  from ShoppingCartItems item inner 
                                    join ShoppingCart cart on item.ShoppingCartId = cart.ID and cart.UserId=@UserId";
        private const string addAllForShoppingCartSql =@"insert into ShoppingCartItems (ShoppingCartId, ProductCatalogId, 
                            ProductName, ProductDescription, Amount, Currency)
                            values (@ShoppingCartId, @ProductCatalogId, @ProductName,@ProductDescription, @Amount, @Currency)";
        public async Task Save(ShoppingCart shoppingCart)
        {
            using (var conn=new MySqlConnection(connectionString))
            {
                using (var tx=conn.BeginTransaction())
                {
                    await conn.ExecuteAsync(deleteAllForShoppingCartSql, new { UserId = shoppingCart.UserId }, tx).ConfigureAwait(false);
                    await conn.ExecuteAsync(addAllForShoppingCartSql, shoppingCart.Items, tx).ConfigureAwait(false);
                }
            }
        }
    }
}
