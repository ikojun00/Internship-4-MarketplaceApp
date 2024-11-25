using Internship_4_MarketplaceApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship_4_MarketplaceApp.Classes
{
    public class Seller : User
    {
        private double TotalEarnings { get; set; }

        public Seller(string name, string email) : base(name, email)
        {
            TotalEarnings = 0;
        }
        
        public void AddEarnings(double amount)
        {
            TotalEarnings += amount;
        }
    }
}
