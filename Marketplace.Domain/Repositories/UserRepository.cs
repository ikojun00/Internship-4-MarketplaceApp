using Marketplace.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Domain.Repositories
{
    public class UserRepository
    {
        private readonly MarketplaceContext _context;

        public UserRepository(MarketplaceContext context)
        {
            _context = context;
        }

        public bool IsEmailValid(string email)
        {
            var valid = true;

            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }

            return valid;
        }
    }
}
