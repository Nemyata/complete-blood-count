using System.ComponentModel.DataAnnotations;


namespace BloodCount.Web;

public class ExtensionsAttribute(string[] extensions) : ValidationAttribute
{
    private readonly string[] _extensions = extensions;

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not IFormFile file)
            return new ValidationResult("Входной объект не файл");

        var extension = Path.GetExtension(file.FileName);
        if (!_extensions.Contains(extension.ToLower()))
            return new ValidationResult($"Расшерение файла не соответсвует: *{string.Join(" *", _extensions)}");

        return ValidationResult.Success;
    }
}