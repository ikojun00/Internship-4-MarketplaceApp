using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship_4_MarketplaceApp.Classes
{
    public class Seller : User
    {
        private List<Product> Products { get; set; }
        private double TotalEarnings { get; set; }

        public Seller(string name, string email) : base(name, email)
        {
            Products = new List<Product>();
            TotalEarnings = 0;
        }
    }
}
