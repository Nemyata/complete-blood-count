using System.Text.Json.Serialization;

namespace BloodCount.Domain;

public class LLM2ML
{
    [JsonPropertyName("Пол")]
    public string Gender { get; set; }

    [JsonPropertyName("Гемоглобин")]
    public double Hemoglobin { get; set; }

    [JsonPropertyName("Эритроциты")]
    public double RedBloodCells { get; set; }

    [JsonPropertyName("Тромбоциты")]
    public double Platelets { get; set; }

    [JsonPropertyName("Лейкоциты")]
    public double Leukocytes { get; set; }
}