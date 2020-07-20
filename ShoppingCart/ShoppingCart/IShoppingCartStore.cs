namespace ShoppingCart.ShoppingCart
{
    public interface IShoppingCartStore
    {
        ShoppingCart Get(int userId);
        void Save(object shoppingCart);
    }
}