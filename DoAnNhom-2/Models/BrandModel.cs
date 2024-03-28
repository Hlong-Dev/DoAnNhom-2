using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnNhom_2.Models
{
    [Table("Brand")]
    public class BrandModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yeu cau nhap ten thuong hieu danh muc")]
        public string Name { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yeu cau nhap mo ta danh muc")]
        public string Description { get; set; }
        public string Slug { get; set; }
        public int Status { get; set; }
    }
}
