using System.Text.Json.Serialization;

namespace MagicPlayExporter.Models.Import;

public class GameMetaData
{
    [JsonPropertyName("GameAddedBoards")]
    public List<string> GameAddedBoards { get; set; } = new();

    [JsonPropertyName("GameAddedLocations")]
    public List<string> GameAddedLocations { get; set; } = new();

    [JsonPropertyName("GameAddedDecks")]
    public List<string> GameAddedDecks { get; set; } = new();
}
