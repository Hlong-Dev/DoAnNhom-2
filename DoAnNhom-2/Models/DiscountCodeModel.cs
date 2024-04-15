using System;

namespace DoAnNhom_2.Models
{
    public class DiscountCodeModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Discount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime ExpiryDate { get; set; } // Ngày hết hạn
        public DateTime CreationDate { get; set; } // Ngày tạo
        public int Quantity { get; set; } // Số lượng còn lại

        // Constructor để khởi tạo ngày tạo mặc định
        public DiscountCodeModel()
        {
            CreationDate = DateTime.Now;
        }
    }
}
