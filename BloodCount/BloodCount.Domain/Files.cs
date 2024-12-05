namespace BloodCount.Domain;

public class Files
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string OCR { get; set; }

    public LLM2ML LLM { get; set; }
}