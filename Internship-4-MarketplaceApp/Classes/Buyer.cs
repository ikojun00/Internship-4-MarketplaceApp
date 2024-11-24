using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship_4_MarketplaceApp.Classes
{
    public class Buyer : User
    {
        private double Balance { get; set; }
        private List<Product> PurchasedProducts { get; set; }
        private List<Product> FavoriteProducts { get; set; }

        public Buyer(string name, string email, double initialBalance) : base(name, email)
        {
            Balance = initialBalance;
            PurchasedProducts = new List<Product>();
            FavoriteProducts = new List<Product>();
        }
    }
}
