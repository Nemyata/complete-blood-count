namespace BloodCount.Domain;

public class LLM
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public string Gender { get; set; }
    public int Hemoglobin { get; set; }
    public int RedBloodCells { get; set; }
    public int Platelets { get; set; }
    public int Leukocytes { get; set; }
    public string ResultML { get; set; }

    public Files Files { get; set; }
}