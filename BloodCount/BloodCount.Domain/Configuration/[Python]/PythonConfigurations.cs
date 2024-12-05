using System.ComponentModel;

namespace BloodCount.Domain.Configuration;

public class PythonConfigurations
{
    public string Python { get; set; }
    public OCRConfig OCR { get; set; }
    public LLMConfig LLM { get; set; }
    public MLConfig ML { get; set; }

    public string GetArguments(PythonType type, string args)
    {
        string path;
        string arg = $"\"{args}\"";
        switch (type)
        {
            case PythonType.OCR:
            string tes = $"\"{OCR.Tesseract}\"";
            path = string.Format("{0} {1}", OCR.Script, string.Join(" ", tes, arg));
            break;

            case PythonType.LLM:
            string apiKey = $"{LLM.APIKey}";
            path = string.Format("{0} {1}", LLM.Script, string.Join(" ", apiKey, arg));
            break;

            case PythonType.ML:
            path = string.Format("{0} {1} {2}", ML.Script, arg, ML.Path);
            break;

            default:
            throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(PythonType));
        }
        return path;
    }
}