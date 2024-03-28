using System.ComponentModel.DataAnnotations;

namespace DoAnNhom_2.Models.ViewModels
{
	public class LoginViewModel
	{
        

        public int Id { get; set; }
		[Required(ErrorMessage = "Nhap UserName")]
		public string Username { get; set; }
		
		[DataType(DataType.Password), Required(ErrorMessage = "Nhap Password")]
		public string Password { get; set; }
		public string ReturnUrl { get; set; }
 
    }
}
