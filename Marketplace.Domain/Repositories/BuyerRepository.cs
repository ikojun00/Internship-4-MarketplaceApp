using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain.Repositories
{
    public class BuyerRepository
    {
        private readonly MarketplaceContext _context;

        public BuyerRepository(MarketplaceContext context)
        {
            _context = context;
        }

        public void AddToFavorites(Buyer buyer, Product product)
        {
            buyer.FavoriteProducts.Add(product);
        }

        public bool PurchaseProduct(Buyer buyer, Product product, double finalPrice)
        {
            if (buyer.Balance >= finalPrice && product.Status == ProductStatus.ForSale)
            {
                buyer.Balance -= finalPrice;
                var purchasedProduct = new Product(
                    product.Name,
                    product.Description,
                    finalPrice,
                    product.Seller,
                    product.Category
                );
                buyer.PurchasedProducts.Add(purchasedProduct);
                product.Status = ProductStatus.Sold;
                return true;
            }
            return false;
        }

        public void ReturnProduct(Buyer buyer, Product product, double buyerCut)
        {
            buyer.Balance += buyerCut;
            buyer.PurchasedProducts.Remove(product);
        }

    }
}
