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
    public class MainMenu
    {
        private readonly MarketplaceRepository _marketplace;

        public MainMenu(MarketplaceRepository marketplace)
        {
            _marketplace = marketplace;
        }

        public void ShowMainMenu()
        {
            string[] options = {
                "Izlaz",
                "Prijava",
                "Registracija",
            };

            Helper.ShowMenu(Array.Empty<string>(), options, choice =>
            {
                switch (choice)
                {
                    case 1: LoginUserInterface(); break;
                    case 2: RegisterUserInterface(); break;
                }
            });
        }

        private void LoginUserInterface()
        {
            Console.Clear();
            Console.Write("Ime: ");
            string name = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Clear();
            var user = _marketplace.LoginUser(name, email);
            if (user != null)
            {
                if (user is Buyer buyer)
                    new BuyerMenu(_marketplace, buyer).ShowBuyerMenu();
                else if (user is Seller seller)
                    new SellerMenu(_marketplace, seller).ShowSellerMenu();
            }
            else Console.WriteLine("Neuspješna prijava.");
        }

        private void RegisterUserInterface()
        {
            Console.Clear();
            string name = Helper.GetValidatedString("Ime: ", "Ime korisnika ne može biti prazno.\n");
            string email = "";
            while (true)
            {
                Console.Write("Email: ");
                email = Console.ReadLine();
                if (!_marketplace.IsEmailValid(email))
                {
                    Console.WriteLine("Email je nevažeći.\n");
                    continue;
                }
                if (_marketplace.EmailExists(email))
                {
                    Console.WriteLine("Email se već koristi.\n");
                    continue;
                }
                break;
            }

            while (true)
            {
                Console.Write("Unesite 'k' za kupca ili 'p' za prodavača: ");
                string choice = Console.ReadLine().ToLower();

                switch (choice)
                {
                    case "k":
                        double balance = 0;
                        while (true)
                        {
                            Console.Write("Stanje računa: ");
                            if (double.TryParse(Console.ReadLine(), out balance) && balance > 0) break;
                            Console.WriteLine("Unesite valjanu cijenu veću od 0.\n");
                        }
                        _marketplace.RegisterBuyer(name, email, balance);
                        Console.WriteLine("\nKupac uspješno registriran!");
                        break;
                    case "p":
                        _marketplace.RegisterSeller(name, email);
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