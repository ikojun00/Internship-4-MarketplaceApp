using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain.Repositories
{
    public class MarketplaceRepository
    {
        private readonly MarketplaceContext _context;
        private readonly BuyerRepository _buyerRepository;
        private readonly SellerRepository _sellerRepository;
        private const double CommissionRate = 0.05;

        public MarketplaceRepository(MarketplaceContext context)
        {
            _context = context;
            _buyerRepository = new BuyerRepository(context);
            _sellerRepository = new SellerRepository(context);
        }
        public User LoginUser(string name, string email)
        {
            return _context.Users.Find(u => u.Name == name && u.Email == email);
        }

        public Buyer RegisterBuyer(string name, string email, double initialBalance)
        {
            if (_context.Users.Exists(u => u.Email == email))
                throw new InvalidOperationException($"Postoji korisnik s email-om '{email}'. Izaberite neki drugi email.\n");
            var buyer = new Buyer(name, email, initialBalance);
            _context.Users.Add(buyer);
            return buyer;
        }

        public Seller RegisterSeller(string name, string email)
        {
            if (_context.Users.Exists(u => u.Email == email))
                throw new InvalidOperationException($"Postoji korisnik s email-om '{email}'. Izaberite neki drugi email.\n");
            var seller = new Seller(name, email);
            _context.Users.Add(seller);
            return seller;
        }

        public double UsePromoCode(Product product, string promoCode)
        {
            double finalPrice = product.Price;

            if (!string.IsNullOrEmpty(promoCode) &&
                _context.PromoCodes.TryGetValue(promoCode, out var promoDetails) &&
                promoDetails.Category == product.Category &&
                promoDetails.ExpiryDate > DateTime.Now)
            {
                finalPrice *= (1 - promoDetails.DiscountPercentage);
            }

            return finalPrice;
        }
        public void AddPromoCode(string code, ProductCategory category, DateTime expiryDate, double discountPercentage)
        {
            _context.PromoCodes[code] = (category, expiryDate, discountPercentage);
        }

        public void AddProduct(string name, string description, double price, Seller seller, ProductCategory category)
        {
            var product = new Product(name, description, price, seller, category);
            _context.Products.Add(product);
        }

        public List<Product> GetAvailableProducts()
        {
            return _context.Products.Where(p => p.Status == ProductStatus.ForSale).ToList();
        }

        public List<Product> FilterByCategory(ProductCategory category)
        {
            return _context.Products.Where(p => p.Category == category && p.Status == ProductStatus.ForSale).ToList();
        }

        public List<Product> GetSellerProducts(Seller seller)
        {
            return _context.Products.Where(p => p.Seller == seller).ToList();
        }

        public List<Product> GetSoldProductsByCategory(Seller seller, ProductCategory category)
        {
            return _context.Products.Where(p => p.Seller == seller && p.Category == category && p.Status == ProductStatus.Sold).ToList();
        }

        public double GetEarningsInPeriod(Seller seller, DateTime startDate, DateTime endDate)
        {
            return _context.Transactions.Where(t => t.Seller == seller && t.Product.Status == ProductStatus.Sold
                && t.Date >= startDate && t.Date <= endDate).Sum(t => t.Amount);
        }

        public bool ProcessTransaction(Product product, Buyer buyer, double finalPrice)
        {
            if (_buyerRepository.PurchaseProduct(buyer, product, finalPrice))
            {
                var commission = finalPrice * CommissionRate;
                var sellerAmount = finalPrice - commission;
                _sellerRepository.AddEarnings(product.Seller, sellerAmount);
                var transaction = new Transaction(product, buyer, product.Seller, finalPrice);
                _context.Transactions.Add(transaction);
                return true;
            }
            return false;
        }

        public bool ProcessTransactionReturn(Product product)
        {
            var realProduct = _context.Products.Find(p => p.Seller == product.Seller && p.Name == product.Name);
            var transaction = _context.Transactions.Find(t => t.Product == realProduct);
            if (transaction != null)
            {
                var buyerCut = transaction.Amount * 0.8;
                var sellerCut = transaction.Amount * 0.85;
                _buyerRepository.ReturnProduct(transaction.Buyer, product, buyerCut);
                _sellerRepository.AddEarnings(transaction.Seller, -sellerCut);
                realProduct.Status = ProductStatus.ForSale;
                _context.Transactions.Remove(transaction);
                return true;
            }
            return false;
        }
    }
}
