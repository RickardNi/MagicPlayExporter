namespace MagicPlayExporter.Models.Export;

public class Game
{
    public int Id { get; set; }
    public int BggId { get; set; }
    public string BggName { get; set; } = string.Empty;
    public int BggYear { get; set; }
    public bool Cooperative { get; set; }
    public string Designers { get; set; } = string.Empty;
    public bool HighestWins { get; set; }
    public int IsBaseGame { get; set; }
    public int IsExpansion { get; set; }
    public string ModificationDate { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Uuid { get; set; } = string.Empty;
}
