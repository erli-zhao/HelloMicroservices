using ShoppingCart.EventFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.ShoppingCart
{
    public class ShoppingCart
    {
        private HashSet<ShoppingCartItem> items = new HashSet<ShoppingCartItem>();
        
        public int UserId { get; }

        public IEnumerable<ShoppingCartItem> Items { get { return items; } set { } }

        public ShoppingCart(int userId, IEnumerable<ShoppingCartItem> item)
        {
            this.UserId = userId;
            this.Items = item;
        }

        public void AddItems(IEnumerable<ShoppingCartItem> shoppingCartItems, IEventStore eventStore)
        {
            foreach (var item in shoppingCartItems)
            {
                if (this.items.Add(item))
                {
                    eventStore.Raise("ShoppingCartItemAdded", new { UserId, item });
                }
            }
        }

        public void RemoveItems(int[] productCatalogueIds, IEventStore eventStore)
        {
            items.RemoveWhere(i => productCatalogueIds.Contains(i.ProductCatalogueId));
        }
    }
}
