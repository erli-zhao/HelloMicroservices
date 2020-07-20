namespace ShoppingCart.ShoppingCart
{
    public class ShoppingCartItem
    {
        public int ProductCatalogueId { get; }
        public string ProductName { get; }
        public string Desscription { get; }
        public Money Price { get; }

        public ShoppingCartItem(
            int productCatalogueId,
            string productName,
            string description,
            Money price)
        {
            this.ProductCatalogueId = productCatalogueId;
            this.ProductName = productName;
            this.Desscription = description;
            this.Price = price;
        }

        public override bool Equals(object obj)
        {
            if (obj==null||GetType()!=obj.GetType())
            {
                return false;
            }
            var that = obj as ShoppingCartItem;
            return this.ProductCatalogueId.Equals(that.ProductCatalogueId);
        }

        public override int GetHashCode()
        {
            return this.ProductCatalogueId.GetHashCode();  
        }
    }

    public class Money
    {
        public string Currency { get; }
        public decimal Amount { get; }

        public Money(string currency, decimal amount)
        {
            this.Currency = currency;
            this.Amount = amount;
        }
    }
}
