using Internship_4_MarketplaceApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship_4_MarketplaceApp.Classes
{
    public class Buyer : User
    {
        public double Balance { get; private set; }
        public List<Product> PurchasedProducts { get; private set; }
        public List<Product> FavoriteProducts { get; private set; }

        public Buyer(string name, string email, double initialBalance) : base(name, email)
        {
            Balance = initialBalance;
            PurchasedProducts = new List<Product>();
            FavoriteProducts = new List<Product>();
        }

        public void AddToFavorites(Product product)
        {
            FavoriteProducts.Add(product);
        }

        public bool PurchaseProduct(Product product, double finalPrice)
        {
            if (Balance >= finalPrice && product.Status == ProductStatus.ForSale)
            {
                Balance -= finalPrice;
                var purchasedProduct = new Product(
                    product.Name,
                    product.Description,
                    finalPrice,
                    product.Seller,
                    product.Category
                );
                PurchasedProducts.Add(purchasedProduct);
                product.Status = ProductStatus.Sold;
                return true;
            }
            return false;
        }

        public void ReturnProduct(Product product, double buyerCut)
        {
            Balance += buyerCut;
            PurchasedProducts.Remove(product);
        }
    }
}
