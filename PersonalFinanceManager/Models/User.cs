using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceManager.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    }

}
