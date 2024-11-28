using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using Marketplace.Domain.Repositories;
using System;
using System.Collections.Generic;
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
            ShowMainMenu();
        }
        private static void DisplayProducts(List<Product> products, string message)
        {
            if (products.Count == 0)
            {
                Console.WriteLine(message);
                return;
            }

            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.Id}");
                Console.WriteLine($"Naziv: {product.Name}");
                Console.WriteLine($"Opis: {product.Description}");
                Console.WriteLine($"Cijena: {product.Price} eura");
                Console.WriteLine($"Kategorija: {product.Category}");
                Console.WriteLine($"Status: {product.Status}");
                Console.WriteLine(new string('-', 50));
            }
        }
        private static ProductCategory SelectCategory()
        {
            ProductCategory category;
            while (true)
            {
                Console.WriteLine("1 - Elektronika");
                Console.WriteLine("2 - Odjeća i obuća");
                Console.WriteLine("3 - Knjige");
                Console.WriteLine("4 - Ostalo");
                Console.Write("Odaberi kategoriju: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        category = ProductCategory.Electronics;
                        break;
                    case "2":
                        category = ProductCategory.Clothing;
                        break;
                    case "3":
                        category = ProductCategory.Books;
                        break;
                    case "4":
                        category = ProductCategory.Other;
                        break;
                    default:
                        Console.WriteLine("Nepostojeća opcija. Pokušajte ponovno.");
                        continue;
                }
                break;
            }
            return category;
        }
        private static void DisplayProductsByCategory()
        {
            var category = SelectCategory();
            var products = marketplace.FilterByCategory(category);
            Console.Clear();
            DisplayProducts(products, "Trenutno nema dostupnih proizvoda za prodaju.");
        }
        private static void DisplayPurchaseHistory(Buyer buyer)
        {
            var purchaseHistory = buyer.PurchasedProducts;
            DisplayProducts(purchaseHistory, "Nemate kupljenih proizvoda.");
        }
        private static void DisplayAvailableProducts()
        {
            var products = marketplace.GetAvailableProducts();
            DisplayProducts(products, "Trenutno nema dostupnih proizvoda za prodaju.");
        }
        private static void DisplayFavoriteProducts(Buyer buyer)
        {
            var favorites = buyer.FavoriteProducts;
            DisplayProducts(favorites, "Nemate omiljenih proizvoda.");
        }
        private static void PurchaseProduct(Buyer buyer)
        {
            Console.Write("Unesite ID proizvoda koji želite kupiti: ");
            var productId = Console.ReadLine();
            var productToBuy = marketplace.GetAvailableProducts()
                .FirstOrDefault(p => p.Id == productId);

            if (productToBuy == null)
            {
                Console.WriteLine("Proizvod nije pronađen.");
                return;
            }

            Console.WriteLine($"\nOdabrani proizvod");
            Console.WriteLine($"Naziv: {productToBuy.Name}");
            Console.WriteLine($"Cijena: {productToBuy.Price} eura");
            /*
            Console.Write("\nŽelite li dodati u favorite? (da/ne): ");
            if (Console.ReadLine().ToLower() == "da")
            {
                buyer.AddToFavorites(productToBuy);
                Console.WriteLine("Proizvod dodan u favorite.");
            }*/

            double price = productToBuy.Price;
            Console.Write("\nŽelite li iskoristiti promo kod? (da/ne): ");
            if (Console.ReadLine().ToLower() == "da")
            {
                while (true)
                {
                    Console.Write("\nPromo kod: ");
                    string promoCode = Console.ReadLine();
                    price = marketplace.UsePromoCode(productToBuy, promoCode);
                    if (price == productToBuy.Price)
                    {
                        Console.WriteLine("\nUneseni promo kod ne vrijedi.");
                        Console.Write("Želite li iskoristiti drugi promo kod? (da/ne): ");
                        if (Console.ReadLine().ToLower() == "da") continue;
                        break;
                    }
                    break;
                }
                Console.WriteLine($"\nCijena nakon korištenja promo koda");
                Console.WriteLine($"Naziv: {productToBuy.Name}");
                Console.WriteLine($"Cijena: {price} eura");
            }

            Console.Write("\nPotvrđujete kupnju? (da/ne): ");
            if (Console.ReadLine().ToLower() != "da")
            {
                Console.WriteLine("Kupnja otkazana.");
                return;
            }
            bool success = marketplace.ProcessTransaction(productToBuy, buyer, price);
            Console.WriteLine(success
                ? "\nKupnja uspješno izvršena!"
                : "\nKupnja nije uspjela. Provjerite stanje računa ili dostupnost proizvoda.");
        }
        private static void ReturnProduct(Buyer buyer)
        {
            Console.Write("Unesite ID proizvoda koji želite vratiti: ");
            var productId = Console.ReadLine();

            var productToReturn = buyer.PurchasedProducts.FirstOrDefault(p => p.Id == productId);

            if (productToReturn == null)
            {
                Console.WriteLine("Proizvod nije pronađen.");
                return;
            }

            Console.WriteLine($"\nOdabrani proizvod");
            Console.WriteLine($"Naziv: {productToReturn.Name}");
            Console.WriteLine($"Cijena: {productToReturn.Price} eura");

            Console.Write("\nPotvrđujete povrat? (da/ne): ");
            if (Console.ReadLine().ToLower() != "da")
            {
                Console.WriteLine("Povrat otkazan.");
                return;
            }

            bool success = marketplace.ProcessTransactionReturn(productToReturn);
            Console.WriteLine(success
                ? "Proizvod uspješno vraćen."
                : "Povrat proizvoda nije uspio.");
        }
        private static void ShowBuyerManagement(User user)
        {
            var buyer = (Buyer)user;
            Console.Clear();
            while (true)
            {
                try
                {
                    Console.WriteLine($"Dobrodošli, {buyer.Name}\n");
                    Console.WriteLine($"Imate: {buyer.Balance} eura\n");
                    Console.WriteLine("1 - Pregled svih proizvoda na prodaji");
                    Console.WriteLine("2 - Pregled proizvoda po kategoriji");
                    Console.WriteLine("3 - Pregled kupljenih proizvoda");
                    Console.WriteLine("4 - Pregled omiljenih proizvoda");
                    Console.WriteLine("5 - Kupnja proizvoda");
                    Console.WriteLine("6 - Povrat kupljenog proizvoda");
                    Console.WriteLine("0 - Odjava");

                    Console.Write("\nOdabir: ");
                    string choice = Console.ReadLine();
                    Console.Clear();
                    switch (choice)
                    {
                        case "1":
                            DisplayAvailableProducts();
                            break;
                        case "2":
                            DisplayProductsByCategory();
                            break;
                        case "3":
                            DisplayPurchaseHistory(buyer);
                            break;
                        case "4":
                            DisplayFavoriteProducts(buyer);
                            break;
                        case "5":
                            PurchaseProduct(buyer);
                            break;
                        case "6":
                            ReturnProduct(buyer);
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

        public static void AddNewProduct(Seller seller)
        {
            string name = "";
            while (true)
            {
                Console.Write("Ime proizvoda: ");
                name = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(name)) break;
                Console.WriteLine("Ime proizvoda ne može biti prazno.\n");
            }

            string description = "";
            while (true)
            {
                Console.Write("Opis proizvoda: ");
                description = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(description)) break;
                Console.WriteLine("Opis proizvoda ne može biti prazan.\n");
            }

            double price = 0;
            while (true)
            {
                Console.Write("Cijena proizvoda: ");
                if (double.TryParse(Console.ReadLine(), out price) && price > 0) break;
                Console.WriteLine("Unesite valjanu cijenu veću od 0.\n");
            }

            ProductCategory category = SelectCategory();

            marketplace.AddProduct(name, description, price, seller, category);
            Console.WriteLine("\nProizvod uspješno dodan.");
        }

        private static void DisplaySellerProducts(Seller seller)
        {
            var products = marketplace.GetSellerProducts(seller);
            DisplayProducts(products, "Nemate proizvoda na prodaji.");
        }

        private static void DisplaySoldProductsByCategory(Seller seller)
        {
            ProductCategory category = SelectCategory();
            var soldProducts = marketplace.GetSoldProductsByCategory(seller, category);
            Console.Clear();
            DisplayProducts(soldProducts, "Nema prodanih proizvoda u odabranoj kategoriji.");
        }

        private static void DisplayEarningsInTimePeriod(Seller seller)
        {
            DateTime startDate, endDate;

            Console.Write("Unesite početni datum (dd.MM.yyyy.): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy.", null, System.Globalization.DateTimeStyles.None, out startDate))
            {
                Console.WriteLine("Nevažeći format datuma.");
                return;
            }

            Console.Write("Unesite završni datum (dd.MM.yyyy.): ");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy.", null, System.Globalization.DateTimeStyles.None, out endDate))
            {
                Console.WriteLine("Nevažeći format datuma.");
                return;
            }

            if (startDate > endDate)
            {
                Console.WriteLine("Datum početka ne može biti nakon datuma kraja.");
                return;
            }

            var earningsInPeriod = marketplace.GetEarningsInPeriod(seller, startDate, endDate);

            Console.WriteLine($"\nZarada u razdoblju od {startDate:dd.MM.yyyy} do {endDate:dd.MM.yyyy}: {earningsInPeriod} eura");
        }

        private static void ShowSellerManagement(User user)
        {
            var seller = (Seller)user;
            Console.Clear();
            while (true)
            {
                try
                {
                    Console.WriteLine($"Dobrodošli, {seller.Name}\n");
                    Console.WriteLine($"Vaša zarada: {seller.TotalEarnings} eura\n");
                    Console.WriteLine("1 - Dodaj novi proizvod");
                    Console.WriteLine("2 - Pregled vlastitih proizvoda");
                    Console.WriteLine("3 - Pregled prodanih proizvoda po kategoriji");
                    Console.WriteLine("4 - Pregled zarade u vremenskom razdoblju");
                    Console.WriteLine("0 - Odjava");

                    Console.Write("\nOdabir: ");
                    string choice = Console.ReadLine();
                    Console.Clear();
                    switch (choice)
                    {
                        case "1":
                            AddNewProduct(seller);
                            break;
                        case "2":
                            DisplaySellerProducts(seller);
                            break;
                        case "3":
                            DisplaySoldProductsByCategory(seller);
                            break;
                        case "4":
                            DisplayEarningsInTimePeriod(seller);
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

            marketplace.AddProduct("iPhone 14", "Najnoviji iPhone model", 799.99, seller, ProductCategory.Electronics);
            marketplace.AddProduct("Tenisice za trčanje", "Nike tenisice za trčanje", 69.99, seller, ProductCategory.Clothing);
            marketplace.AddProduct("Knjiga programiranja", "Naučite C# za 30 dana", 49.99, seller, ProductCategory.Books);

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
            string name = "";
            while (true)
            {
                Console.Write("Ime: ");
                name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Ime korisnika ne može biti prazno.\n");
                    continue;
                }
                break;
            }

            while (true)
            {
                Console.Write("Email: ");
                string email = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("Email korisnika ne može biti prazno.\n");
                    continue;
                }
                try
                {
                    Console.Write("Pritisnite 'k' za registraciju kao kupac ili 'p' kao prodavač: ");
                    string choice = Console.ReadLine().ToLower();

                    switch (choice)
                    {
                        case "k":
                            marketplace.RegisterBuyer(name, email, 1000);
                            Console.WriteLine("\nKupac uspješno registriran!");
                            return;
                        case "p":
                            marketplace.RegisterSeller(name, email);
                            Console.WriteLine("\nProdavač uspješno registriran!");
                            return;
                        default:
                            Console.WriteLine("Nevažeći odabir. Molimo odaberite 'k' ili 'p'.");
                            break;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
