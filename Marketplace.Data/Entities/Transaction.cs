using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities
{
    public class Transaction
    {
        public string Id { get; private set; }
        public Product Product { get; private set; }
        public Buyer Buyer { get; private set; }
        public Seller Seller { get; private set; }
        public DateTime Date { get; private set; }
        public double Amount { get; private set; }

        public Transaction(Product product, Buyer buyer, Seller seller, double amount)
        {
            Id = Guid.NewGuid().ToString();
            Product = product;
            Buyer = buyer;
            Seller = seller;
            Date = DateTime.Now;
            Amount = amount;
        }
    }
