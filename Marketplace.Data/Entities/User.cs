using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities
{
    public abstract class User
    {
        public string Name { get; protected set; }
        public string Email { get; protected set; }

        public User(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
}
