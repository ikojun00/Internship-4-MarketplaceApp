using Internship_4_MarketplaceApp.Menus;
using Marketplace.Data;
using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using Marketplace.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship_4_MarketplaceApp
{
    public class Program
    {
        private static MarketplaceRepository marketplace;

        static void Main(string[] args)
        {
            var context = new MarketplaceContext();
            marketplace = new MarketplaceRepository(context);
            
            SeedData();

            var mainMenu = new MainMenu(marketplace);
            mainMenu.ShowMainMenu();
        }

        private static void SeedData()
        {
            var seller = marketplace.RegisterSeller("Ivo", "ivo@gmail.com");
            var buyer = marketplace.RegisterBuyer("Ante", "ante@gmail.com", 1000);

            marketplace.AddProduct("iPhone 14", "Najnoviji iPhone model", 799.99, seller, ProductCategory.Electronics);
            marketplace.AddProduct("Tenisice za trčanje", "Nike tenisice za trčanje", 69.99, seller, ProductCategory.Clothing);
            marketplace.AddProduct("Knjiga programiranja", "Naučite C# za 30 dana", 49.99, seller, ProductCategory.Books);

            marketplace.AddPromoCode("SUMMER2024", ProductCategory.Clothing, DateTime.Now.AddMonths(3), 0.20);
            marketplace.AddPromoCode("TECHDEALS", ProductCategory.Electronics, DateTime.Now.AddMonths(1), 0.15);
        }
    }
}
