namespace MagicPlayExporter.Models;

public class FormData
{
    public DateTime? PlayDate { get; set; }
    public string SelectedFormat { get; set; } = "Draft";
    public string SelectedSet { get; set; } = string.Empty;
    public string SelectedDraftType { get; set; } = string.Empty;
    public List<GameplayRowData> GameplayRows { get; set; } = new();
    public Dictionary<string, HashSet<string>> PlayerColorsMap { get; set; } = new();
}

public class GameplayRowData
{
    public int? Player1Id { get; set; }
    public int? Player2Id { get; set; }
    public string StartingPlayer { get; set; } = string.Empty;
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string Player1Deck { get; set; } = string.Empty;
    public string Player2Deck { get; set; } = string.Empty;
    public string Winner { get; set; } = string.Empty;
    public HashSet<string> P1Colors { get; set; } = new();
    public HashSet<string> P2Colors { get; set; } = new();
}
