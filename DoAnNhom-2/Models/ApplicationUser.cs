using DoAnNhom_2.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string Fullname { get; set; }
    public string? Address { get; set; }
    public int Age { get; set; }
    public bool? IsDeleted { get; set; }
    public ICollection<OrderDetails> OrderDetails { get; set; }
    public Cart Cart { get; set; }
}