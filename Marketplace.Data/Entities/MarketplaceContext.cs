using Marketplace.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities
{
    public class MarketplaceContext
    {
        public List<User> Users { get; set; }
        public List<Product> Products { get; set; }
        public List<Transaction> Transactions { get; set; }
        public Dictionary<string, (ProductCategory Category, DateTime ExpiryDate, double DiscountPercentage)> PromoCodes { get; set; }

        public MarketplaceContext()
        {
            Users = new List<User>();
            Products = new List<Product>();
            Transactions = new List<Transaction>();
            PromoCodes = new Dictionary<string, (ProductCategory, DateTime, double)>();
        }
    }
}
