using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DoAnNhom_2.Repository.Validation;

namespace DoAnNhom_2.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
    
        public string Name { get; set; }
       
        public string Slug { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yeu cau nhap mo ta danh muc")]
        public string Description { get; set; }
        [Required(ErrorMessage = "yeu cau nhap gia san pham")]
       
        public decimal Price { get; set; }

   

        public decimal OldPrice { get; set; } // Giá cũ
        public bool IsDeleted { get; set; } = false; // Indicates if the category has been soft-deleted, default is false.
        public void SoftDelete()
        {
            IsDeleted = true;
            // Bạn có thể cập nhật thêm các trường như Ngày xóa, Người xóa,...
        }
        public int BrandId {  get; set; }
        public BrandModel Brand { get; set; }
        public int CategoryId { get; set; }
        public CategoryModel Category { get; set; }

       
        public string Image { get; set; } = "noimage.jpg";
        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload  { get; set; }
        public int Quantity { get; set; }

  
        public string Size { get; set; }
    }
   
}
