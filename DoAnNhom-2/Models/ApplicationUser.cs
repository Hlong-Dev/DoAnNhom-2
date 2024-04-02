using Microsoft.AspNetCore.Identity;
using System;

namespace DoAnNhom_2
{
    public class ApplicationUser : IdentityUser
    {
        // Thêm thuộc tính Address
        public virtual string Address { get; set; }

        // Constructors
        public ApplicationUser() : base() { }

        public ApplicationUser(string userName) : base(userName) { }
    }
}
