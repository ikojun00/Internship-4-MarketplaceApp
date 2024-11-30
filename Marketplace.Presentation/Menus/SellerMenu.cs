using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using Marketplace.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship_4_MarketplaceApp.Menus
{
    public class SellerMenu
    {
        private readonly MarketplaceRepository _marketplace;
        private readonly Seller _seller;

        public SellerMenu(MarketplaceRepository marketplace, Seller seller)
        {
            _marketplace = marketplace;
            _seller = seller;
        }

        public void ShowSellerMenu()
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine($"Dobrodošli, {_seller.Name}\n");
                Console.WriteLine($"Vaša zarada: {_seller.TotalEarnings} eura\n");
                Console.WriteLine("0 - Odjava");
                Console.WriteLine("1 - Dodaj novi proizvod");
                Console.WriteLine("2 - Pregled vlastitih proizvoda");
                Console.WriteLine("3 - Pregled prodanih proizvoda po kategoriji");
                Console.WriteLine("4 - Pregled zarade u vremenskom razdoblju");
                Console.Write("\nOdabir: ");
                string choice = Console.ReadLine();
                Console.Clear();
                switch (choice)
                {
                    case "0":
                        Console.WriteLine("Odjava.");
                        return;
                    case "1":
                        AddNewProduct();
                        break;
                    case "2":
                        DisplaySellerProducts();
                        break;
                    case "3":
                        DisplaySoldProductsByCategory();
                        break;
                    case "4":
                        DisplayEarningsInTimePeriod();
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

        private void AddNewProduct()
        {
            string name = Helper.GetValidatedString("Ime proizvoda: ", "Ime proizvoda ne može biti prazno.\n");
            string description = Helper.GetValidatedString("Opis proizvoda: ", "Opis proizvoda ne može biti prazan.\n");

            double price = 0;
            while (true)
            {
                Console.Write("Cijena proizvoda: ");
                if (double.TryParse(Console.ReadLine(), out price) && price > 0) break;
                Console.WriteLine("Unesite valjanu cijenu veću od 0.\n");
            }
            Console.WriteLine();
            ProductCategory category = Helper.SelectCategory();

            _marketplace.AddProduct(name, description, price, _seller, category);
            Console.WriteLine("\nProizvod uspješno dodan.");
        }

        private void DisplaySellerProducts()
        {
            var products = _marketplace.GetSellerProducts(_seller);
            Helper.DisplayProducts(products, "Nemate proizvoda na prodaji.");
        }

        private void DisplaySoldProductsByCategory()
        {
            ProductCategory category = Helper.SelectCategory();
            var soldProducts = _marketplace.GetSoldProductsByCategory(_seller, category);
            Console.Clear();
            Helper.DisplayProducts(soldProducts, "Nema prodanih proizvoda u odabranoj kategoriji.");
        }

        private void DisplayEarningsInTimePeriod()
        {
            DateTime startDate, endDate;
            while (true)
            {
                startDate = Helper.GetValidDate("Unesite početni datum (dd.MM.yyyy.): ");
                endDate = Helper.GetValidDate("Unesite završni datum (dd.MM.yyyy.): ");
                if (startDate <= endDate) break;
                Console.WriteLine("Datum početka ne može biti nakon datuma kraja.");
            }

            var earningsInPeriod = _marketplace.GetEarningsInPeriod(_seller, startDate, endDate);
            Console.WriteLine($"\nZarada u razdoblju od {startDate:dd.MM.yyyy} do {endDate:dd.MM.yyyy}: {earningsInPeriod} eura");
        }
    }
}
