using System.Text.Json.Serialization;

namespace MagicPlayExporter.Models.Import;

public class GameMetaData
{
    [JsonPropertyName("CollectionHistory")]
    public Dictionary<string, int> CollectionHistory { get; set; } = new();

    [JsonPropertyName("GameAddedRoles")]
    public List<string> GameAddedRoles { get; set; } = new();

    [JsonPropertyName("GameAddedBoards")]
    public List<string> GameAddedBoards { get; set; } = new();
}
