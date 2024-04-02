using System;
using Microsoft.AspNetCore.Identity;

namespace DoAnNhom_2
{
    // Tạo một lớp mới kế thừa từ lớp IdentityUser và thêm các trường bạn muốn
    public class ExtendedIdentityUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }

        public ExtendedIdentityUser() : base() { }

        public ExtendedIdentityUser(string userName) : base(userName) { }
    }
}
