namespace DoAnNhom_2.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string OrderCode { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string FullName { get; set; } // Họ và tên
        public string Address { get; set; } // Địa chỉA
        public string PhoneNumber { get; set; } // Số điện thoại
        public DateTime CreatedDate { get; set; }
    }
}
