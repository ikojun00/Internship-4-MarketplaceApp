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
        private static Dictionary<ProductCategory, string> categoryTranslations = new Dictionary<ProductCategory, string>
        {
            { ProductCategory.Electronics, "Elektronika" },
            { ProductCategory.Clothing, "Odjeća i obuća" },
            { ProductCategory.Books, "Knjige" },
            { ProductCategory.Other, "Ostalo" }
        };
        private static Dictionary<ProductStatus, string> statusTranslations = new Dictionary<ProductStatus, string>
        {
            { ProductStatus.Sold, "Prodano" },
            { ProductStatus.ForSale, "U prodaji" },
        };

        static void Main(string[] args)
        {
            var context = new MarketplaceContext();
            marketplace = new MarketplaceRepository(context);
            SeedData();
            ShowMainMenu();
        }
        private static void ShowMenu(string[] options, Action<int> handleSelection)
        {
            while (true)
            {
                Console.Clear();
                for (int i = 0; i < options.Length; i++) Console.WriteLine($"{i} - {options[i]}");
                Console.Write("\nOdabir: ");

                if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice < options.Length)
                {
                    Console.Clear();
                    if (choice == 0) return;
                    handleSelection(choice);
                }
                else Console.WriteLine("Nepostojeća opcija. Pokušajte ponovno.");
                
                Console.Write("\nPritisnite bilo koju tipku za nastavak...");
                Console.ReadKey();
                Console.Clear();
            }
        }
        private static DateTime GetValidDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy.", null, System.Globalization.DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }
                Console.WriteLine("Nevažeći format datuma. Pokušajte ponovno.");
            }
        }

        private static string GetValidatedString(string prompt, string errorMessage)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) return input;
                Console.WriteLine(errorMessage);
            }
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
                Console.WriteLine($"Kategorija: {categoryTranslations[product.Category]}");
                Console.WriteLine($"Status: {statusTranslations[product.Status]}");
                Console.WriteLine(new string('-', 50));
            }
        }
        private static ProductCategory SelectCategory()
        {
            foreach (var value in Enum.GetValues(typeof(ProductCategory)))
            {
                Console.WriteLine($"{(int)value} - {categoryTranslations[(ProductCategory)value]}");
            }
            while (true)
            {
                Console.Write("Odaberi kategoriju: ");
                if (int.TryParse(Console.ReadLine(), out int selected) &&
                    Enum.IsDefined(typeof(ProductCategory), selected))
                {
                    return (ProductCategory)selected;
                }
                Console.WriteLine("Nepostojeća opcija. Pokušajte ponovno.");
            }
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
            
            Console.Write("\nŽelite li dodati u favorite? (da/ne): ");
            if (Console.ReadLine().ToLower() == "da")
            {
                marketplace.AddToFavorites(buyer, productToBuy);
                Console.WriteLine("Proizvod dodan u favorite.");
            }

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
            string[] buyerOptions = {
                "Odjava",
                "Pregled svih proizvoda na prodaji",
                "Pregled proizvoda po kategoriji",
                "Pregled kupljenih proizvoda",
                "Pregled omiljenih proizvoda",
                "Kupnja proizvoda",
                "Povrat kupljenog proizvoda"
            };

            ShowMenu(buyerOptions, choice =>
            {
                switch (choice)
                {
                    case 1: DisplayAvailableProducts(); break;
                    case 2: DisplayProductsByCategory(); break;
                    case 3: DisplayPurchaseHistory(buyer); break;
                    case 4: DisplayFavoriteProducts(buyer); break;
                    case 5: PurchaseProduct(buyer); break;
                    case 6: ReturnProduct(buyer); break;
                }
            });
        }


        public static void AddNewProduct(Seller seller)
        {
            string name = GetValidatedString("Ime proizvoda: ", "Ime proizvoda ne može biti prazno.\n");
            string description = GetValidatedString("Opis proizvoda: ", "Opis proizvoda ne može biti prazan.\n");

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
            while(true)
            {
                startDate = GetValidDate("Unesite početni datum (dd.MM.yyyy.): ");
                endDate = GetValidDate("Unesite završni datum (dd.MM.yyyy.): ");
                if (startDate <= endDate) break;
                Console.WriteLine("Datum početka ne može biti nakon datuma kraja.");
            }

            var earningsInPeriod = marketplace.GetEarningsInPeriod(seller, startDate, endDate);

            Console.WriteLine($"\nZarada u razdoblju od {startDate:dd.MM.yyyy} do {endDate:dd.MM.yyyy}: {earningsInPeriod} eura");
        }

        private static void ShowSellerManagement(User user)
        {
            var seller = (Seller)user;
            Console.WriteLine($"Dobrodošli, {seller.Name}\n");
            Console.WriteLine($"Vaša zarada: {seller.TotalEarnings} eura\n");
            string[] sellerOptions = {
                "Odjava",
                "Dodaj novi proizvod",
                "Pregled vlastitih proizvoda",
                "Pregled prodanih proizvoda po kategoriji",
                "Pregled zarade u vremenskom razdoblju",
            };

            ShowMenu(sellerOptions, choice =>
            {
                switch (choice)
                {
                    case 1: AddNewProduct(seller); break;
                    case 2: DisplaySellerProducts(seller); break;
                    case 3: DisplaySoldProductsByCategory(seller); break;
                    case 4: DisplayEarningsInTimePeriod(seller); break;
                }
            });
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
            string name = GetValidatedString("Ime: ", "Ime korisnika ne može biti prazno.\n");
            string email = "";
            while (true)
            {
                Console.Write("Email: ");
                email = Console.ReadLine();
                if (!marketplace.IsEmailValid(email))
                {
                    Console.WriteLine("Email je nevažeći.\n");
                    continue;
                }
                if (marketplace.EmailExists(email))
                {
                    Console.WriteLine("Email se već koristi.\n");
                    continue;
                }
                break;
            }

            while (true)
            {
                Console.Write("Pritisnite 'k' za registraciju kao kupac ili 'p' kao prodavač: ");
                string choice = Console.ReadLine().ToLower();

                switch (choice)
                {
                    case "k":
                        double balance = 0;
                        while (true)
                        {
                            Console.Write("Cijena proizvoda: ");
                            if (double.TryParse(Console.ReadLine(), out balance) && balance > 0) break;
                            Console.WriteLine("Unesite valjanu cijenu veću od 0.\n");
                        }
                        marketplace.RegisterBuyer(name, email, balance);
                        Console.WriteLine("\nKupac uspješno registriran!");
                        break;
                    case "p":
                        marketplace.RegisterSeller(name, email);
                        Console.WriteLine("\nProdavač uspješno registriran!");
                        break;
                    default:
                        Console.WriteLine("Nevažeći odabir. Molimo odaberite 'k' ili 'p'.");
                        continue;
                }
                break;
            }
        }
    }
}
