using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Internship_4_MarketplaceApp
{
    public class Helper
    {
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

        public static void ShowMenu(string[] options, Action<int> handleSelection)
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

        public static DateTime GetValidDate(string prompt)
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

        public static string GetValidatedString(string prompt, string errorMessage)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) return input;
                Console.WriteLine(errorMessage);
            }
        }

        public static void DisplayProducts(List<Product> products, string message)
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

        public static ProductCategory SelectCategory()
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
    }
}