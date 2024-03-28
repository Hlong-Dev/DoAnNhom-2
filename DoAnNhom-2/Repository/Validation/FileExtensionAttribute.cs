using System.ComponentModel.DataAnnotations;
namespace DoAnNhom_2.Repository.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                string[] extensions = { "jpg","png","jpeg" };

                bool result = extensions.Any(x => extension.EndsWith(x));
                if(!result)
                {
                    return new ValidationResult("Allowed extensions are jpg,png,jpeg");
                }
            }
            return ValidationResult.Success;
        }

    }
}
