using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities
{
    public class Buyer : User
    {
        public double Balance { get; set; }
        public List<Product> PurchasedProducts { get; private set; }
        public List<Product> FavoriteProducts { get; private set; }

        public Buyer(string name, string email, double initialBalance) : base(name, email)
        {
            Balance = initialBalance;
            PurchasedProducts = new List<Product>();
            FavoriteProducts = new List<Product>();
        }
    }
}
