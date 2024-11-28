using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities
{
    public class Seller : User
    {
        public double TotalEarnings { get; set; }
        public Seller(string name, string email) : base(name, email)
        {
            TotalEarnings = 0;
        }
    }
}
