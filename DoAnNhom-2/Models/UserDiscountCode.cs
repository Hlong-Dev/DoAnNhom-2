using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnNhom_2.Models
{
    public class UserDiscountCode
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(UserId))]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(DiscountCodeId))]
        public int DiscountCodeId { get; set; }
        public DiscountCodeModel DiscountCode { get; set; }
    }

}
