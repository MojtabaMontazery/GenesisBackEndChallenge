using System;
using System.Collections.Generic;

namespace DALCore.DB
{
    public partial class Users
    {
        public Users()
        {
            UsersPhones = new HashSet<UsersPhones>();
        }

        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public DateTime LastLoginOn { get; set; }
        public string Token { get; set; }

        public ICollection<UsersPhones> UsersPhones { get; set; }
    }
}
