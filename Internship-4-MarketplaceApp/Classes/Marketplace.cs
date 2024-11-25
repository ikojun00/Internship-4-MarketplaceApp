using Internship_4_MarketplaceApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship_4_MarketplaceApp.Classes
{
    public class Marketplace
    {
        private List<User> Users { get; set; }
        private List<Product> Products { get; set; }
        private List<Transaction> Transactions { get; set; }
        private Dictionary<string, (ProductCategory Category, DateTime ExpiryDate, double DiscountPercentage)> PromoCodes { get; set; }
        private const double CommissionRate = 0.05;

        public Marketplace()
        {
            Users = new List<User>();
            Products = new List<Product>();
            Transactions = new List<Transaction>();
            PromoCodes = new Dictionary<string, (ProductCategory, DateTime, double)>();
        }

        public User LoginUser(string name, string email)
        {
            return Users.Find(u => u.Name == name && u.Email == email);
        }

        public Buyer RegisterBuyer(string name, string email, double initialBalance)
        {
            var buyer = new Buyer(name, email, initialBalance);
            Users.Add(buyer);
            return buyer;
        }

        public Seller RegisterSeller(string name, string email)
        {
            var seller = new Seller(name, email);
            Users.Add(seller);
            return seller;
        }

        public void AddPromoCode(string code, ProductCategory category, DateTime expiryDate, double discountPercentage)
        {
            PromoCodes[code] = (category, expiryDate, discountPercentage);
        }

        public void AddProduct(string name, string description, double price, Seller seller, ProductCategory category)
        {
            var product = new Product(name, description, price, seller, category);
            Products.Add(product);
        }

        public List<Product> GetAvailableProducts()
        {
            return Products.Where(p => p.Status == ProductStatus.ForSale).ToList();
        }

        public List<Product> FilterByCategory(ProductCategory category)
        {
            return Products.Where(p => p.Category == category && p.Status == ProductStatus.ForSale).ToList();
        }

        public bool ProcessTransaction(Product product, Buyer buyer, string promoCode = null)
        {
            double finalPrice = product.Price;

            if (!string.IsNullOrEmpty(promoCode) &&
                PromoCodes.TryGetValue(promoCode, out var promoDetails) &&
                promoDetails.Category == product.Category &&
                promoDetails.ExpiryDate > DateTime.Now)
            {
                finalPrice *= (1 - promoDetails.DiscountPercentage);
            }

            if (buyer.PurchaseProduct(product))
            {
                var commission = finalPrice * CommissionRate;
                var sellerAmount = finalPrice - commission;
                product.Seller.AddEarnings(sellerAmount);
                var transaction = new Transaction(product, buyer, product.Seller, finalPrice);
                Transactions.Add(transaction);

                return true;
            }

            return false;
        }
    }
}
