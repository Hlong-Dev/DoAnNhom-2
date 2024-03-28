using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnNhom_2.Models
{
    [Table("Category")]
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yeu cau nhap ten danh muc")]

        public string Name { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yeu cau nhap mo ta danh muc")]
        public string Description { get; set; }

        [Required]
        public string Slug { get; set; }
        public int Status { get; set; }
        public int? ParentId { get; set; } // ParentId cho biết mục cha của danh mục hiện tại
     
        public bool IsDeleted { get; set; } = false; // Indicates if the category has been soft-deleted, default is false.
        public CategoryModel ParentCategory { get; set; } // Tham chiếu tới danh mục cha
      

    }
}
