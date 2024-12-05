using System.ComponentModel.DataAnnotations;

namespace BloodCount.Web.Model;

public class FileViewModel
{
    [DataType(DataType.Upload)]
    [Required(ErrorMessage = "Файл не выбран")]
    [Extensions([".jpg", ".jpeg", ".gif", ".png", ".tiff", ".pdf"])]
    public IFormFile File { get; set; }
}