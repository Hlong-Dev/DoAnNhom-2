namespace DoAnNhom_2.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string OrderCode { get; set; }
        public long ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string FullName { get; set; } // Họ và tên
        public string Address { get; set; } // Địa chỉ
        public string PhoneNumber { get; set; } // Số điện thoại
    }
}
