using System;
using System.Collections.Generic;

namespace DALCore.DB
{
    public partial class UsersPhones
    {
        public long UserPhoneId { get; set; }
        public Guid UserId { get; set; }
        public string Phone { get; set; }

        public Users User { get; set; }
    }
}
