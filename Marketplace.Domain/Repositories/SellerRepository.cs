using Marketplace.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain.Repositories
{
    public class SellerRepository
    {
        private readonly MarketplaceContext _context;

        public SellerRepository(MarketplaceContext context)
        {
            _context = context;
        }
        public void AddEarnings(Seller seller, double amount)
        {
            seller.TotalEarnings += amount;
        }
    }
}
