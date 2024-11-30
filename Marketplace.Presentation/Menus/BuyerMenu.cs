using Marketplace.Data.Entities;
using Marketplace.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship_4_MarketplaceApp.Menus
{
    public class BuyerMenu
    {
        private readonly MarketplaceRepository _marketplace;
        private readonly Buyer _buyer;

        public BuyerMenu(MarketplaceRepository marketplace, Buyer buyer)
        {
            _marketplace = marketplace;
            _buyer = buyer;
        }

        public void ShowBuyerMenu()
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine($"Dobrodošli, {_buyer.Name}\n");
                Console.WriteLine($"Imate: {_buyer.Balance} eura\n");
                Console.WriteLine("0 - Odjava");
                Console.WriteLine("1 - Pregled svih proizvoda na prodaji");
                Console.WriteLine("2 - Pregled proizvoda po kategoriji");
                Console.WriteLine("3 - Pregled kupljenih proizvoda");
                Console.WriteLine("4 - Pregled omiljenih proizvoda");
                Console.WriteLine("5 - Kupnja proizvoda");
                Console.WriteLine("6 - Povrat kupljenog proizvoda");
                Console.Write("\nOdabir: ");
                string choice = Console.ReadLine();
                Console.Clear();
                switch (choice)
                {
                    case "0":
                        Console.WriteLine("Odjava.");
                        return;
                    case "1":
                        DisplayAvailableProducts();
                        break;
                    case "2":
                        DisplayProductsByCategory();
                        break;
                    case "3":
                        DisplayPurchaseHistory();
                        break;
                    case "4":
                        DisplayFavoriteProducts();
                        break;
                    case "5":
                        PurchaseProduct();
                        break;
                    case "6":
                        ReturnProduct();
                        break;
                    default:
                        Console.WriteLine("Nepostojeća opcija. Pokušajte ponovno.");
                        break;
                }
                
                Console.Write("\nPritisnite bilo koju tipku za nastavak...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private void DisplayAvailableProducts()
        {
            var products = _marketplace.GetAvailableProducts();
            Helper.DisplayProducts(products, "Trenutno nema dostupnih proizvoda za prodaju.");
        }

        private void DisplayProductsByCategory()
        {
            var category = Helper.SelectCategory();
            var products = _marketplace.FilterByCategory(category);
            Console.Clear();
            Helper.DisplayProducts(products, "Trenutno nema dostupnih proizvoda za prodaju.");
        }

        private void DisplayPurchaseHistory()
        {
            var purchaseHistory = _buyer.PurchasedProducts;
            Helper.DisplayProducts(purchaseHistory, "Nemate kupljenih proizvoda.");
        }

        private void DisplayFavoriteProducts()
        {
            var favorites = _buyer.FavoriteProducts;
            Helper.DisplayProducts(favorites, "Nemate omiljenih proizvoda.");
        }

        private void PurchaseProduct()
        {
            Console.Write("Unesite ID proizvoda koji želite kupiti: ");
            var productId = Console.ReadLine();
            var productToBuy = _marketplace.GetAvailableProducts()
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
                _marketplace.AddToFavorites(_buyer, productToBuy);
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
                    price = _marketplace.UsePromoCode(productToBuy, promoCode);
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

            bool success = _marketplace.ProcessTransaction(productToBuy, _buyer, price);
            Console.WriteLine(success
                ? "\nKupnja uspješno izvršena!"
                : "\nKupnja nije uspjela. Provjerite stanje računa ili dostupnost proizvoda.");
        }

        private void ReturnProduct()
        {
            Console.Write("Unesite ID proizvoda koji želite vratiti: ");
            var productId = Console.ReadLine();

            var productToReturn = _buyer.PurchasedProducts.FirstOrDefault(p => p.Id == productId);

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

            bool success = _marketplace.ProcessTransactionReturn(productToReturn);
            Console.WriteLine(success
                ? "Proizvod uspješno vraćen."
                : "Povrat proizvoda nije uspio.");
        }
    }
}
