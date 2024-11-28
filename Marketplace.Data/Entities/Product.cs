using Marketplace.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities
{
    public class Product
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public double Price { get; private set; }
        public ProductStatus Status { get; set; }
        public Seller Seller { get; private set; }
        public ProductCategory Category { get; private set; }

        public Product(string name, string description, double price, Seller seller, ProductCategory category)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Description = description;
            Price = price;
            Status = ProductStatus.ForSale;
            Seller = seller;
            Category = category;
        }
    }
}
