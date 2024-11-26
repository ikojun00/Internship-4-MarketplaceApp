﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internship_4_MarketplaceApp.Classes
{
    public abstract class User
    {
        public string Name { get; protected set; }
        public string Email { get; protected set; }
        protected List<Transaction> Transactions { get; set; }

        public User(string name, string email)
        {
            Name = name;
            Email = email;
            Transactions = new List<Transaction>();
        }

    }
}
