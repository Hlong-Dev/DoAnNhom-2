using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DoAnNhom_2.Models
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<CartItemModel> CartItems { get; set;}
    }
}
