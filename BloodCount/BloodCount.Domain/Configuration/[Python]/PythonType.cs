using System.ComponentModel.DataAnnotations;

namespace BloodCount.Domain.Configuration;

public enum PythonType : int
{
    [Display(Name = "OCR")]
    OCR = 0,

    [Display(Name = "LLM")]
    LLM = 1,

    [Display(Name = "ML")]
    ML = 2,
}