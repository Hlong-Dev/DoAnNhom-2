using System.ComponentModel.DataAnnotations;

namespace DoAnNhom_2.Models
{
	public class CartItemModel
	{
        [Key]
        public long ProductId { get; set; }
		public string ProductName { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
        public decimal Discount { get; set; } // Thêm thuộc tính Discount để lưu giá trị giảm giá cho từng mặt hàng
        public string FullName { get; set; }
        public string Address { get; set; }
        public decimal Total
        {
            get { return Quantity * (Price - Discount); } // Áp dụng giảm giá khi tính tổng
        }
        public string? ImageUrl { get; set; }
		public CartItemModel()
		{
		}
		public CartItemModel(ProductModel product)
		{
			ProductId = product.Id;
			ProductName = product.Name;
			Price = product.Price;
			Quantity = 1;
			ImageUrl = product.Image;
            Discount = 0;
            FullName = string.Empty;
            Address = string.Empty;
        }
	}
}
