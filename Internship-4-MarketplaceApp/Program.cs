using Internship_4_MarketplaceApp.Classes;
using Internship_4_MarketplaceApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship_4_MarketplaceApp
{
    public class Program
    {
        private static Marketplace marketplace;
        static void Main(string[] args)
        {
            marketplace = new Marketplace();
            SeedData();
            ShowMainMenu();
        }
        private static void ShowBuyerManagement(User user)
        {
            var buyer = (Buyer)user;
            Console.Clear();
            while (true)
            {
                try
                {
                    Console.WriteLine("1 - Pregled svih proizvoda dostupnih za prodaju");
                    Console.WriteLine("2 - Kupnja proizvoda unosom ID-a proizvoda");
                    Console.WriteLine("0 - Odjava");

                    Console.Write("\nOdabir: ");
                    string choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            Console.Clear();
                            var products = marketplace.GetAvailableProducts();
                            if (products.Count == 0) Console.WriteLine("Trenutno nema dostupnih proizvoda za prodaju.");
                            else
                            {
                                Console.WriteLine("Dostupni proizvodi\n");
                                foreach (var product in products)
                                {
                                    Console.WriteLine($"ID: {product.Id}");
                                    Console.WriteLine($"Naziv: {product.Name}");
                                    Console.WriteLine($"Opis: {product.Description}");
                                    Console.WriteLine($"Cijena: {product.Price} eura");
                                    Console.WriteLine($"Kategorija: {product.Category}");
                                    Console.WriteLine("------------------------");
                                }
                            }
                            break;
                        case "2":
                            Console.Write("Unesite ID proizvoda koji želite kupiti: ");
                            var productId = Console.ReadLine();
                            var productToBuy = marketplace.GetAvailableProducts()
                                .FirstOrDefault(p => p.Id == productId);

                            Console.WriteLine($"\nOdabrani proizvod");
                            Console.WriteLine($"Naziv: {productToBuy.Name}");
                            Console.WriteLine($"Cijena: {productToBuy.Price} eura");

                            Console.Write("\nŽelite li koristiti promo kod? (da/ne): ");
                            string usePromoCode = Console.ReadLine().ToLower();
                            string promoCode = null;

                            if (usePromoCode == "da")
                            {
                                Console.Write("Unesite promo kod: ");
                                promoCode = Console.ReadLine();
                            }

                            Console.Write("\nPotvrđujete kupnju? (da/ne): ");
                            string confirm = Console.ReadLine().ToLower();

                            if (confirm == "da")
                            {
                                bool success = marketplace.ProcessTransaction(productToBuy, buyer, promoCode);
                                if (success)
                                    Console.WriteLine("\nKupnja uspješno izvršena!");
                                else
                                    Console.WriteLine("\nKupnja nije uspjela. Provjerite stanje računa ili dostupnost proizvoda.");
                            }
                            else Console.WriteLine("\nKupnja otkazana.");
                            break;
                        case "0":
                            Console.WriteLine("Odjava.");
                            return;
                        default:
                            Console.WriteLine("Nepostojeća opcija. Pokušajte ponovno.");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n" + e.Message);
                }

                Console.Write("\nPritisnite bilo koju tipku za nastavak...");
                Console.ReadKey();
                Console.Clear();
            }
    }
        private static void ShowSellerManagement(User user)
        {

        }
        private static void ShowUserManagement(User user)
        {
            if (user is Buyer)
                ShowBuyerManagement(user);
            else if (user is Seller)
                ShowSellerManagement(user);
        }
        private static void ShowMainMenu()
        {
            while (true)
            {
                Console.WriteLine("1 - Prijava");
                Console.WriteLine("2 - Registracija");
                Console.WriteLine("0 - Izlaz");
                Console.Write("\nOdabir: ");
                string choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        LoginUserInterface();
                        break;
                    case "2":
                        RegisterUserInterface();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Nepostojeća opcija. Pokušajte ponovno.");
                        break;
                }

                Console.Write("\nPritisnite bilo koju tipku za nastavak...");
                Console.ReadKey();
                Console.Clear();
            }
        }
        private static void SeedData()
        {
            var seller = marketplace.RegisterSeller("Ivo", "ivo@gmail.com");
            var buyer = marketplace.RegisterBuyer("Ante", "ante@gmail.com", 1000);

            marketplace.AddProduct("iPhone 14", "Latest iPhone model", 799.99, seller, ProductCategory.Electronics);
            marketplace.AddProduct("Running Shoes", "Nike running shoes", 69.99, seller, ProductCategory.Clothing);
            marketplace.AddProduct("Programming Book", "Learn C# in 30 days", 49.99, seller, ProductCategory.Books);

            marketplace.AddPromoCode("SUMMER2024", ProductCategory.Clothing, DateTime.Now.AddMonths(3), 0.20);
            marketplace.AddPromoCode("TECHDEALS", ProductCategory.Electronics, DateTime.Now.AddMonths(1), 0.15);
        }
        private static void LoginUserInterface() 
        {
            Console.Clear();
            Console.Write("Ime: ");
            string name = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();
            
            Console.Clear();
            var user = marketplace.LoginUser(name, email);
            if (user != null) ShowUserManagement(user);
            else Console.WriteLine("Neuspješna prijava.");
        }
        private static void RegisterUserInterface()
        {
            Console.Clear();
        }
    }
}
