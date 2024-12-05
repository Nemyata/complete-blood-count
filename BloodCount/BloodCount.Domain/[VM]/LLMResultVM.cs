using System.Text.Json.Serialization;

namespace BloodCount.Domain;

public class LLMResultVM
{
    [JsonPropertyName("Пол")]
    public string Gender { get; set; }

    [JsonPropertyName("Гемоглобин")]
    public string Hemoglobin {  get; set; }

    [JsonPropertyName("Эритроциты")]
    public string RedBloodCells { get; set; }

    [JsonPropertyName("Тромбоциты")]
    public string Platelets { get; set; }

    [JsonPropertyName("Лейкоциты")]
    public string Leukocytes { get; set; }
}